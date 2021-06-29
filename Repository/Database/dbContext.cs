using Microsoft.EntityFrameworkCore;
using Repository.Interceptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Repository.Database
{
    public class dbContext : DbContext
    {


        public static string ConnectionString { get; set; }



        public dbContext(DbContextOptions<dbContext> options = default) : base(options = GetDbContextOptions())
        {
        }



        public DbSet<TAlipayKey> TAlipayKey { get; set; }


        public DbSet<TArticle> TArticle { get; set; }


        public DbSet<TCategory> TCategory { get; set; }


        public DbSet<TChannel> TChannel { get; set; }


        public DbSet<TCount> TCount { get; set; }


        public DbSet<TDictionary> TDictionary { get; set; }


        public DbSet<TFile> TFile { get; set; }


        public DbSet<TFileGroup> TFileGroup { get; set; }


        public DbSet<TFileGroupFile> TFileGroupFile { get; set; }


        public DbSet<TGuidToInt> TGuidToInt { get; set; }


        public DbSet<TImgBaiduAI> TImgBaiduAI { get; set; }


        public DbSet<TLink> TLink { get; set; }


        public DbSet<TLog> TLog { get; set; }


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

            //optionsBuilder.UseSqlServer(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseMySQL(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 25)), o => o.MigrationsHistoryTable("__efmigrationshistory"));
            //optionsBuilder.UseSqlite(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));
            optionsBuilder.UseNpgsql(ConnectionString, o => o.MigrationsHistoryTable("__efmigrationshistory"));


            //开启调试拦截器
            //optionsBuilder.AddInterceptors(new DeBugInterceptor());


            //开启数据分表拦截器
            //optionsBuilder.AddInterceptors(new SubTableInterceptor());


            //开启全局懒加载
            //optionsBuilder.UseLazyLoadingProxies();


            return optionsBuilder.Options;
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //循环关闭所有表的级联删除功能
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }


            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name, builder =>
                {

                    //设置生成数据库时的表名为小写格式并添加前缀 t_
                    builder.ToTable("t_" + entity.ClrType.Name.ToLower().Substring(1));


                    //设置表的备注
                    builder.HasComment(GetEntityComment(entity.Name));


                    foreach (var property in entity.GetProperties())
                    {
                        //设置字段名为小写
                        property.SetColumnName(property.Name.ToLower());

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

                    }
                });
            }
        }




        private string GetEntityComment(string typeName, string fieldName = null, List<string> baseTypeNames = null)
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

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == matchKey.ToLower()).Value;
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

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == fieldName.ToLower()).Value;
            }


        }

    }
}
