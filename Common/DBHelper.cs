using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml;

namespace Common
{
    public static class DBHelper
    {

        /// <summary>
        /// 针对数据库执行自定义的sql查询，返回泛型List，可自定义数据库
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">自定义查询Sql</param>
        /// <param name="parameters">sql参数</param>
        /// <param name="connection">数据库连接</param>
        /// <remarks>connection = db.Database.GetDbConnection()</remarks>
        /// <returns></returns>
        public static IList<T> SelectFromSql<T>(string sql, Dictionary<string, object> parameters = default, DbConnection connection = default) where T : class
        {

            if (connection == default)
            {
                using (var db = new dbContext())
                {
                    connection = db.Database.GetDbConnection();
                }
            }

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandTimeout = 600;

            command.CommandText = sql;

            if (parameters != default)
            {
                foreach (var item in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value;

                    command.Parameters.Add(parameter);
                }
            }

            var reader = command.ExecuteReader();

            var dataTable = new DataTable();

            dataTable.Load(reader);

            reader.Close();
            command.Dispose();
            connection.Close();

            var list = DataHelper.DataTableToList<T>(dataTable);

            return list;
        }





        /// <summary>
        /// 针对数据库执行自定义的sql
        /// </summary>
        /// <param name="sql">自定义查询Sql</param>
        /// <param name="parameters">sql参数</param>
        /// <param name="connection">自定义数据库连接</param>
        /// <remarks>connection = db.Database.GetDbConnection()</remarks>
        /// <returns></returns>
        public static void ExecuteSql(string sql, Dictionary<string, object> parameters = default, DbConnection connection = default)
        {

            if (connection == default)
            {
                using (var db = new dbContext())
                {
                    connection = db.Database.GetDbConnection();
                }
            }

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandTimeout = 600;

            command.CommandText = sql;

            if (parameters != default)
            {
                foreach (var item in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value;

                    command.Parameters.Add(parameter);
                }
            }

            command.ExecuteNonQuery();

            command.Dispose();
            connection.Close();
        }





        /// <summary>
        /// 根据Guid获取Int
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int GuidToInt(Guid guid)
        {
            using (var db = new dbContext())
            {
                var info = db.TGuidToInt.Where(t => t.Guid == guid).FirstOrDefault() ?? new TGuidToInt();

                if (info.Id == 0)
                {
                    info.Guid = guid;

                    var dbinfo = db.TGuidToInt.Add(info);

                    db.SaveChanges();

                    return dbinfo.Entity.Id;
                }
                else
                {
                    return info.Id;
                }
            }
        }




        /// <summary>
        /// 根据Int获取Guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid IntToGuid(int id)
        {
            using (var db = new dbContext())
            {
                return db.TGuidToInt.Where(t => t.Id == id).Select(t => t.Guid).FirstOrDefault();
            }
        }



        /// <summary>
        /// 保存日志信息
        /// </summary>
        /// <param name="Sign">自定义标记</param>
        /// <param name="Type">日志类型</param>
        /// <param name="Content">日志内容</param>
        /// <returns></returns>
        public static bool LogSet(string Sign, string Type, string Content)
        {
            try
            {
                using (var db = new dbContext())
                {
                    var log = new TLog();

                    log.Id = Guid.NewGuid();
                    log.Sign = Sign;
                    log.Type = Type;
                    log.Content = Content;
                    log.CreateTime = DateTime.Now;

                    db.TLog.Add(log);

                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 账户合并方法，仅限SqlServer
        /// </summary>
        /// <param name="oldUserId">原始账户ID</param>
        /// <param name="newUserId">新账户ID</param>
        /// <returns></returns>
        public static bool MergeUser(Guid oldUserId, Guid newUserId)
        {
            try
            {
                using (var db = new dbContext())
                {


                    string sql = "SELECT t.name AS [Key],c.name AS Value FROM sys.tables AS t INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID WHERE c.system_type_id = 231 and c.name LIKE '%userid%'";

                    var list = SelectFromSql<dtoKeyValue>(sql);

                    var parameters = new Dictionary<string, object>();

                    foreach (var item in list)
                    {

                        string upSql = "UPDATE [dbo].[@tableName] SET [@columnName] = @newUserId WHERE [@columnName] = @oldUserId";

                        parameters = new Dictionary<string, object>();
                        parameters.Add("tableName", item.Key.ToString());
                        parameters.Add("columnName", item.Value.ToString());
                        parameters.Add("newUserId", newUserId);
                        parameters.Add("oldUserId", oldUserId);

                        db.Database.ExecuteSqlRaw(upSql, parameters);
                    }

                    string delSql = "DELETE FROM [dbo].[t_user] WHERE [id] = @oldUserId";

                    parameters = new Dictionary<string, object>();
                    parameters.Add("oldUserId", oldUserId);

                    db.Database.ExecuteSqlRaw(delSql, parameters);

                    return true;
                }

            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// 为指定类型的计数 +1
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public static int RunCountSet(string tag)
        {
            using (var db = new dbContext())
            {

                var info = db.TCount.Where(t => t.Tag == tag).FirstOrDefault() ?? new TCount();

                if (info.Id != default)
                {
                    info.Count = info.Count + 1;
                    info.UpdateTime = DateTime.Now;

                    db.SaveChanges();

                    return info.Count;
                }
                else
                {
                    info.Id = Guid.NewGuid();
                    info.Tag = tag;
                    info.Count = 1;
                    info.CreateTime = DateTime.Now;

                    db.TCount.Add(info);

                    db.SaveChanges();

                    return info.Count;

                }

            }
        }



        /// <summary>
        /// 通过类型获取计数值
        /// </summary>
        /// <param name="tag">标记</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public static int RunCountGet(string tag, DateTime starttime = default(DateTime), DateTime endtime = default(DateTime))
        {

            using (var db = new dbContext())
            {
                var query = db.TCount.Where(t => t.Tag.Contains(tag));

                if (starttime != default(DateTime) & endtime != default(DateTime))
                {
                    query = query.Where(t => t.CreateTime >= starttime & t.CreateTime <= endtime);
                }

                return query.ToList().Sum(t => t.Count);
            }

        }




        /// <summary>
        /// 获取一个表的注释信息
        /// </summary>
        /// <typeparam name="T">表的实体类型</typeparam>
        /// <param name="fieldName">字段名称</param>
        /// <remarks>字段名称为空时返回表的注释</remarks>
        /// <returns></returns>
        public static string GetEntityComment<T>(string fieldName = null) where T : new()
        {
            var path = AppContext.BaseDirectory + "/Repository.xml";
            var xml = new XmlDocument();
            xml.Load(path);

            var fieldList = new Dictionary<string, string>();

            var memebers = xml.SelectNodes("/doc/members/member");

            var t = new T();


            if (fieldName == null)
            {
                var matchKey = "T:" + t.ToString();

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
                var baseTypeNames = new List<string>();
                var baseType = t.GetType().BaseType;
                while (baseType != null)
                {
                    baseTypeNames.Add(baseType.FullName);
                    baseType = baseType.BaseType;
                }

                foreach (object m in memebers)
                {
                    if (m is XmlNode node)
                    {
                        var name = node.Attributes["name"].Value;

                        var summary = node.InnerText.Trim();

                        var matchKey = "P:" + t.ToString() + ".";

                        if (name.StartsWith(matchKey))
                        {
                            name = name.Replace("P:" + t.ToString() + ".", "");
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
