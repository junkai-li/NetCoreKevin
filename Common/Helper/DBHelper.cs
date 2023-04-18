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

            //if (connection == default)
            //{
            //    using (var db = new dbContext())
            //    {
            //        connection = db.Database.GetDbConnection();
            //    }
            //}

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

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == matchKey.ToLower()).Value ?? t.ToString().Split(".").ToList().LastOrDefault();
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

                return fieldList.FirstOrDefault(t => t.Key.ToLower() == fieldName.ToLower()).Value ?? fieldName;
            }

        }
    }
}
