
using kevin.Domain.BaseDatas;
using kevin.Domain.Bases;
using kevin.Domain.Entities;
using kevin.Domain.EventBus;
using kevin.Domain.Kevin;
using Kevin.Common.App;
using Kevin.EntityFrameworkCore.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Repository.Interceptors;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TencentCloud.Eb.V20210416.Models;
using Web.Global.User;
namespace Repository.Database
{
    public class dbContext : DbContext
    {

        private IMediator? Mediator;
        public static string ConnectionString { get; set; }

        public Int32 TenantId { get; set; }


        public dbContext(DbContextOptions<dbContext> _ = default, IMediator? mediator = default, ICurrentUser service = default) : base(GetDbContextOptions())
        {
            if (mediator != default)
            {
                this.Mediator = mediator;
            }

            if (service != default)
            {
                this.TenantId = service.TenantId;
            }

        }


        private static DbContextOptions<dbContext> GetDbContextOptions()
        {

            var optionsBuilder = new DbContextOptionsBuilder<dbContext>();


            //SQLServer:"Data Source=localhost;Initial Catalog=webcore;User ID=sa;Password=123456;Max Pool Size=100;Encrypt=False"
            //MySQL:"server=127.0.0.1;database=webcore;user id=root;password=123456;maxpoolsize=100"
            //SQLite:"Data Source=../Repository/database.db"
            //PostgreSQL:"Host=127.0.0.1;Database=webcore;Username=postgres;Password=123456;Maximum Pool Size=100"
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
                //optionsBuilder.UseMySQL(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
                optionsBuilder.UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 22)), o => o.MigrationsHistoryTable("__efmigrationshistory"));
                //optionsBuilder.UseSqlite(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
                //optionsBuilder.UseNpgsql(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            }

            //开启调试拦截器
            optionsBuilder.AddInterceptors(new DeBugInterceptor());


            //开启数据分表拦截器
            //optionsBuilder.AddInterceptors(new SubTableInterceptor());


            //开启全局懒加载
            //optionsBuilder.UseLazyLoadingProxies();


            //SQL日志记录消息写入控制台
            //optionsBuilder.LogTo(Console.WriteLine);

            return optionsBuilder.Options;
        }


        public static List<Type> GetTables()
        {
            Assembly ser = Assembly.GetExecutingAssembly();
            var data = ser.GetTypes().Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && a.GetCustomAttributes<TableAttribute>().Any()).ToList();
            Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in Assemblys)
            {
                data.AddRange(item.GetTypes().Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && a.GetCustomAttributes<TableAttribute>().Any()).ToList());
            } 
            return data;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 自动注入table

            foreach (Type t in GetTables())
            {
                var type = Type.GetType($"{t.FullName},{t.Assembly.FullName}");
                var attribute = type.GetCustomAttribute<TableAttribute>();
                string tableName = attribute == null ? type.Name : attribute.Name;
                if (modelBuilder.Model.FindEntityType(type) == null) modelBuilder.Model.AddEntityType(type);
            }

            #endregion

            //循环关闭所有表的级联删除功能
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //自定义外键
            //  modelBuilder.Entity<TGroupBuyActivityGroupUser>()
            //     .HasOne(p => p.GroupBuyActivityGroup)
            //     .WithMany(b => b.GroupBuyActivityGroupUserList)
            //     .HasForeignKey(p => p.GroupBuyActivityGroupId)
            //     .HasConstraintName("FK_c_groupBuyActivityGroupId");

            //  modelBuilder.Entity<TGroupBuyActivityProduct>()
            //.HasOne(p => p.GroupBuyActivity)
            //.WithMany(b => b.GroupBuyActivityProductList)
            //.HasForeignKey(p => p.GroupBuyActivityId)
            //.HasConstraintName("FK_c_groupBuyActivityIds");

            //  modelBuilder.Entity<TGroupBuyActivityProduct>()
            //  .HasOne(p => p.ProductPackage)
            //  .WithMany()
            //  .HasForeignKey(p => p.ProductPackageId)
            //  .HasConstraintName("FK_c_productPackageIds");
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name, builder =>
                {
                    //设置生成数据库时的表名为小写格式并添加前缀 t_
                    var tableName =("t_" + entity.ClrType.Name[1..]);
                    builder.ToTable(tableName);

                    //开启 PostgreSQL 全库行并发乐观锁
                    //builder.UseXminAsConcurrencyToken();

                    //设置表的备注
                    builder.HasComment(GetEntityComment(entity.Name));
                    //租户过滤器 
                    //Expression<Func<CD, bool>> multiTenantFilter = e => EF.Property<string>(e, "TenantId") == (TenantId ?? TenantHelper.GetSettingsTenantId());

                    //builder.HasQueryFilter(multiTenantFilter);

                    foreach (var property in entity.GetProperties())
                    {
                        string columnName = property.GetColumnName(StoreObjectIdentifier.Create(property.DeclaringEntityType, StoreObjectType.Table).Value);

                        //设置字段名为小写
                        property.SetColumnName(columnName);

                        var baseTypeNames = new List<string>();
                        var baseType = entity.ClrType.BaseType;
                        while (baseType != null)
                        {
                            baseTypeNames.Add(baseType.FullName);
                            baseType = baseType.BaseType;
                        }


                        //设置字段的备注
                        property.SetComment(GetEntityComment(entity.Name, property.Name, baseTypeNames));


                        //bool to bit 使用 MySQL 时需要取消注释
                        if (property.ClrType.Name == typeof(bool).Name)
                        {
                            property.SetColumnType("bit");
                        }


                        //guid to char(36) 使用 MySQL 并且采用 MySql.EntityFrameworkCore 时需要取消注释
                        if (property.ClrType.Name == typeof(Guid).Name)
                        {
                            property.SetColumnType("char(36)");
                        }


                        //为所有 tableid 列添加索引
                        if (property.Name.ToLower() == "tableid")
                        {
                            builder.HasIndex(property.Name);
                        }

                        //设置字段的默认值 
                        var defaultValueAttribute = property.PropertyInfo?.GetCustomAttribute<DefaultValueAttribute>();
                        if (defaultValueAttribute != null)
                        {
                            property.SetDefaultValue(defaultValueAttribute.Value);
                        }

                    }
                });
            }
            #region 种子数据
            modelBuilder.ApplyConfiguration(new TRoleConfiguration());
            modelBuilder.ApplyConfiguration(new TUserConfiguration());
            modelBuilder.ApplyConfiguration(new TTenantConfiguration());
            Console.WriteLine("种子数据已加载");
            #endregion

        }




        public string GetEntityComment(string typeName, string fieldName = null, List<string> baseTypeNames = null)
        {
            var path = AppContext.BaseDirectory + "/kevin.Domain.Share.xml";
            var xml = new XmlDocument();
            xml.Load(path);
            var memebers = xml.SelectNodes("/doc/members/member");

            var fieldList = new Dictionary<string, string>();


            if (fieldName == null)
            {
                var matchKey = "T:" + typeName;

                foreach (object m in memebers)
                {
                    if (m is XmlNode node)
                    {
                        var name = node.Attributes["name"].Value;

                        var summary = node.InnerText.Trim();

                        if (name == matchKey)
                        {
                            fieldList.Add(name, summary);
                        }
                    }
                }

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == matchKey.ToLower()).Value ?? typeName.ToString().Split(".").ToList().LastOrDefault();
            }
            else
            {

                foreach (object m in memebers)
                {
                    if (m is XmlNode node)
                    {
                        var name = node.Attributes["name"].Value;

                        var summary = node.InnerText.Trim();

                        var matchKey = "P:" + typeName + ".";
                        if (name.StartsWith(matchKey))
                        {
                            name = name.Replace(matchKey, "");
                            fieldList.Add(name, summary);
                        }

                        if (baseTypeNames != null)
                        {
                            foreach (var baseTypeName in baseTypeNames)
                            {
                                if (baseTypeName != null)
                                {
                                    matchKey = "P:" + baseTypeName + ".";
                                    if (name.StartsWith(matchKey))
                                    {
                                        name = name.Replace(matchKey, "");
                                        fieldList.Add(name, summary);
                                    }
                                }
                            }
                        }

                    }
                }

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == fieldName.ToLower()).Value ?? fieldName;
            }


        }



        public string ComparisonEntity<T>(T original, T after) where T : new()
        {
            var retValue = "";

            var fields = typeof(T).GetProperties();

            var baseTypeNames = new List<string>();
            var baseType = original.GetType().BaseType;
            while (baseType != null)
            {
                baseTypeNames.Add(baseType.FullName);
                baseType = baseType.BaseType;
            }

            for (int i = 0; i < fields.Length; i++)
            {
                var pi = fields[i];

                string oldValue = pi.GetValue(original)?.ToString();
                string newValue = pi.GetValue(after)?.ToString();

                string typename = pi.PropertyType.FullName;

                if ((typename != "System.Decimal" && oldValue != newValue) || (typename == "System.Decimal" && decimal.Parse(oldValue) != decimal.Parse(newValue)))
                {

                    retValue += GetEntityComment(original.GetType().ToString(), pi.Name, baseTypeNames) + ":";


                    if (pi.Name != "Id" & pi.Name.EndsWith("Id"))
                    {
                        var foreignTable = fields.FirstOrDefault(t => t.Name == pi.Name.Replace("Id", ""));

                        using var db = new dbContext();

                        var foreignName = foreignTable.PropertyType.GetProperties().Where(t => t.CustomAttributes.Where(c => c.AttributeType.Name == "ForeignNameAttribute").Count() > 0).FirstOrDefault();

                        if (foreignName != null)
                        {

                            if (oldValue != null)
                            {
                                var oldForeignInfo = db.Find(foreignTable.PropertyType, Guid.Parse(oldValue));
                                oldValue = foreignName.GetValue(oldForeignInfo).ToString();
                            }

                            if (newValue != null)
                            {
                                var newForeignInfo = db.Find(foreignTable.PropertyType, Guid.Parse(newValue));
                                newValue = foreignName.GetValue(newForeignInfo).ToString();
                            }

                        }

                        retValue += (oldValue ?? "") + " -> ";
                        retValue += (newValue ?? "") + "； \n";

                    }
                    else if (typename == "System.Boolean")
                    {
                        retValue += (oldValue != null ? (bool.Parse(oldValue) ? "是" : "否") : "") + " -> ";
                        retValue += (newValue != null ? (bool.Parse(newValue) ? "是" : "否") : "") + "； \n";
                    }
                    else if (typename == "System.DateTime")
                    {
                        retValue += (oldValue != null ? DateTime.Parse(oldValue).ToString("yyyy-MM-dd") : "") + " ->";
                        retValue += (newValue != null ? DateTime.Parse(newValue).ToString("yyyy-MM-dd") : "") + "； \n";
                    }
                    else
                    {
                        retValue += (oldValue ?? "") + " -> ";
                        retValue += (newValue ?? "") + "； \n";
                    }

                }



            }

            return retValue;
        }


        public override int SaveChanges()
        {

            dbContext db = this;

            #region 发布领域事件
            if (Mediator != default)
            {
                MediatorEventBus(db.ChangeTracker.Entries<IDomainEvents>().Where(x => x.Entity.GetAllDomainEvents().Any()).ToList());
            }
            #endregion


            #region 更改值时处理乐观并发

            var list = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();
            foreach (var item in list)
            {
                item.Entity.GetType().GetProperty("RowVersion")?.SetValue(item.Entity, Guid.NewGuid());
            }

            #endregion

            #region 新增处理多租户

            if (TenantId > 0)
            {
                var Addedlist = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).ToList();
                foreach (var item in Addedlist)
                {
                    item.Entity.GetType().GetProperty("TenantId")?.SetValue(item.Entity, TenantId);
                }
            }

            #endregion

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = false, CancellationToken cancellationToken = default)
        {

            dbContext db = this;

            #region 发布领域事件

            if (Mediator != default)
            {
                MediatorEventBus(db.ChangeTracker.Entries<IDomainEvents>().Where(x => x.Entity.GetAllDomainEvents().Any()).ToList());
            }

            #endregion

            #region 更改值时处理乐观并发

            var list = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();
            foreach (var item in list)
            {
                item.Entity.GetType().GetProperty("RowVersion")?.SetValue(item.Entity, Guid.NewGuid());
            }

            #endregion

            #region 新增处理多租户

            if (TenantId > 0)
            {
                var Addedlist = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).ToList();
                foreach (var item in Addedlist)
                {
                    item.Entity.GetType().GetProperty("TenantId")?.SetValue(item.Entity, TenantId);
                }
            }

            #endregion
            return base.SaveChanges();
        }

        /// <summary>
        /// 领域事件调用
        /// </summary>
        /// <param name="entityEntries"></param>
        public void MediatorEventBus(List<EntityEntry<IDomainEvents>> entityEntries)
        {
            if (Mediator != default)
            {
                foreach (var entityEntry in entityEntries)
                {
                    switch (entityEntry.State)
                    {
                        case EntityState.Deleted:
                            var Deleteddata = entityEntry.Entity.GetDomainEvents(EventBusEnums.Deleted).ToList();
                            entityEntry.Entity.ClearDomainEvents(EventBusEnums.Deleted);
                            Deleteddata.ForEach(e => Mediator.Publish(e));
                            break;
                        case EntityState.Modified:
                            var Modifieddata = entityEntry.Entity.GetDomainEvents(EventBusEnums.Modified).ToList();
                            entityEntry.Entity.ClearDomainEvents(EventBusEnums.Modified);
                            Modifieddata.ForEach(e => Mediator.Publish(e));

                            break;
                        case EntityState.Added:
                            var adddata = entityEntry.Entity.GetDomainEvents(EventBusEnums.Add).ToList();
                            entityEntry.Entity.ClearDomainEvents(EventBusEnums.Add);
                            adddata.ForEach(e => Mediator.Publish(e));

                            break;
                        default:
                            var data = entityEntry.Entity.GetDomainEvents().ToList();
                            entityEntry.Entity.ClearDomainEvents();
                            data.ForEach(e => Mediator.Publish(e));
                            break;
                    }

                }
            }
        }
        public int SaveChangesWithSaveLog(Guid? actionUserId = null, string ipAddress = null, string deviceMark = null)
        {

            dbContext db = this;

            var list = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();
            foreach (var item in list)
            {
                #region 更改值时处理乐观并发
                item.Entity.GetType().GetProperty("RowVersion")?.SetValue(item.Entity, Guid.NewGuid());
                #endregion

                var type = item.Entity.GetType();

                var oldEntity = item.OriginalValues.ToObject();

                var newEntity = item.CurrentValues.ToObject();

                var entityId = item.CurrentValues.GetValue<Guid>("Id");

                if (actionUserId == null)
                {
                    var isHaveUpdateUserId = item.Properties.Where(t => t.Metadata.Name == "UpdateUserId").Count();

                    if (isHaveUpdateUserId > 0)
                    {
                        actionUserId = item.CurrentValues.GetValue<Guid?>("UpdateUserId");
                    }
                }

                var actionUserName = "";

                if (actionUserId != null)
                {
                    actionUserName = db.Set<TUser>().Where(t => t.Id == actionUserId.Value).Select(t => t.Name).FirstOrDefault();
                }

                object[] parameters = { oldEntity, newEntity };

                var result = new dbContext().GetType().GetMethod("ComparisonEntity").MakeGenericMethod(type).Invoke(new dbContext(), parameters);

                if (ipAddress == null | deviceMark == null)
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var httpContextType = assembly.GetTypes().Where(t => t.FullName.Contains("Libraries.Http.HttpContext")).FirstOrDefault();

                    if (httpContextType != null)
                    {
                        if (ipAddress == null)
                        {
                            ipAddress = httpContextType.GetMethod("GetIpAddress", BindingFlags.Public | BindingFlags.Static).Invoke(null, null).ToString();
                        }

                        if (deviceMark == null)
                        {
                            deviceMark = httpContextType.GetMethod("GetHeader", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { "DeviceMark" }).ToString();

                            if (deviceMark == "")
                            {
                                deviceMark = httpContextType.GetMethod("GetHeader", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { "User-Agent" }).ToString();
                            }
                        }
                    }
                }


                var osLog = new TOSLog();
                osLog.Id = Guid.NewGuid();
                osLog.CreateTime = DateTime.Now;
                osLog.Table = type.Name;
                osLog.TableId = entityId;
                osLog.Sign = "Modified";
                osLog.Content = result.ToString();
                osLog.IpAddress = ipAddress == "" ? null : ipAddress;
                osLog.DeviceMark = deviceMark == "" ? null : deviceMark;
                osLog.ActionUserId = actionUserId;
                osLog.TenantId = TenantId;
                db.Set<TOSLog>().Add(osLog);

            }

            #region 新增处理多租户

            var Addedlist = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).ToList();
            foreach (var item in Addedlist)
            {
                item.Entity.GetType().GetProperty("TenantId")?.SetValue(item.Entity, TenantId);
            }

            #endregion
            return base.SaveChanges();
        }


    }
}
