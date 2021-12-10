using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Repository.Interceptors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Repository.Database
{
    public class dbContext : DbContext
    {


        public static string ConnectionString { get; set; }



        public dbContext(DbContextOptions<dbContext> _ = default) : base(GetDbContextOptions())
        {
        }


        public DbSet<TPermission> TPermission { get; set; }

        public DbSet<TRolePermission> TRolePermission { get; set; }

        public DbSet<TAlipayKey> TAlipayKey { get; set; }


        public DbSet<TArticle> TArticle { get; set; }


        public DbSet<TCategory> TCategory { get; set; }


        public DbSet<TChannel> TChannel { get; set; }


        public DbSet<TCount> TCount { get; set; }


        public DbSet<TDictionary> TDictionary { get; set; }


        public DbSet<TFile> TFile { get; set; }


        public DbSet<TFileGroup> TFileGroup { get; set; }


        public DbSet<TFileGroupFile> TFileGroupFile { get; set; }

         


        public DbSet<TImgBaiduAI> TImgBaiduAI { get; set; }


        public DbSet<TLink> TLink { get; set; }


        public DbSet<TLog> TLog { get; set; }

        public DbSet<TOSLog> TOSLog { get; set; }

        public DbSet<TOrder> TOrder { get; set; }


        public DbSet<TOrderDetail> TOrderDetail { get; set; }


        public DbSet<TProduct> TProduct { get; set; }


        public DbSet<TRegionArea> TRegionArea { get; set; }


        public DbSet<TRegionCity> TRegionCity { get; set; }


        public DbSet<TRegionProvince> TRegionProvince { get; set; }

        public DbSet<TRegionTown> TRegionTown { get; set; }



        public DbSet<TRole> TRole { get; set; }


        public DbSet<TSign> TSign { get; set; }


        public DbSet<TUser> TUser { get; set; }


        public DbSet<TUserBindAlipay> TUserBindAlipay { get; set; }


        public DbSet<TUserBindWeixin> TUserBindWeixin { get; set; }


        public DbSet<TUserInfo> TUserInfo { get; set; }


        public DbSet<TUserToken> TUserToken { get; set; }


        public DbSet<TWebInfo> TWebInfo { get; set; }


        public DbSet<TWeiXinKey> TWeiXinKey { get; set; }




        private static DbContextOptions<dbContext> GetDbContextOptions()
        {

            var optionsBuilder = new DbContextOptionsBuilder<dbContext>();


            //SQLServer:"Data Source=127.0.0.1;Initial Catalog=webcore;User ID=sa;Password=123456;Max Pool Size=100"
            //MySQL:"server=127.0.0.1;database=webcore;user id=root;password=123456;maxpoolsize=100"
            //SQLite:"Data Source=../Repository/database.db"
            //PostgreSQL:"Host=127.0.0.1;Database=webcore;Username=postgres;Password=123456;Maximum Pool Size=100"

           optionsBuilder.UseSqlServer(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseMySQL(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 25)), o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseSqlite(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseNpgsql(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));


            //开启调试拦截器
            //optionsBuilder.AddInterceptors(new DeBugInterceptor());


            //开启数据分表拦截器
            //optionsBuilder.AddInterceptors(new SubTableInterceptor());


            //开启全局懒加载
            //optionsBuilder.UseLazyLoadingProxies();


            //SQL日志记录消息写入控制台
            optionsBuilder.LogTo(Console.WriteLine);

            return optionsBuilder.Options;
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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
                    var tableName = builder.Metadata.ClrType.CustomAttributes.Where(t => t.AttributeType.Name == "TableAttribute").Select(t => t.ConstructorArguments.Select(c => c.Value.ToString()).FirstOrDefault()).FirstOrDefault() ?? ("t_" + entity.ClrType.Name[1..]);
                    builder.ToTable(tableName.ToLower());

                    //开启 PostgreSQL 全库行并发乐观锁
                    //builder.UseXminAsConcurrencyToken();

                    //设置表的备注
                    builder.HasComment(GetEntityComment(entity.Name));


                    foreach (var property in entity.GetProperties())
                    {
                        string columnName = property.GetColumnName(StoreObjectIdentifier.Create(property.DeclaringEntityType, StoreObjectType.Table).Value);
                      
                        //设置字段名为小写
                        property.SetColumnName(columnName.ToLower());

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
                        //if (property.ClrType.Name == typeof(bool).Name)
                        //{
                        //    property.SetColumnType("bit");
                        //}


                        //guid to char(36) 使用 MySQL 并且采用 MySql.EntityFrameworkCore 时需要取消注释
                        //if (property.ClrType.Name == typeof(Guid).Name)
                        //{
                        //    property.SetColumnType("char(36)");
                        //}


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
        }




        public string GetEntityComment(string typeName, string fieldName = null, List<string> baseTypeNames = null)
        {
            var path = AppContext.BaseDirectory + "/Repository.xml";
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

            var list = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();

            foreach (var item in list)
            {
                item.Entity.GetType().GetProperty("RowVersion")?.SetValue(item.Entity, Guid.NewGuid());
            }

            return base.SaveChanges();
        }

        public int SaveChangesWithSaveLog(Guid? actionUserId = null, string ipAddress = null, string deviceMark = null)
        {

            dbContext db = this;

            var list = db.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();

            foreach (var item in list)
            {

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
                    actionUserName = db.TUser.Where(t => t.Id == actionUserId.Value).Select(t => t.Name).FirstOrDefault();
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

                db.TOSLog.Add(osLog);

            }

            return db.SaveChanges();
        }

    }
}
