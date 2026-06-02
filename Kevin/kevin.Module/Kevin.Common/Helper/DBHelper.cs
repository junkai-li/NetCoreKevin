using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml;
using MySqlConnector;

namespace Common
{
    public class DBHelper
    {

        private string _defaultConnectionString;

        private string _dbType;

        public DBHelper(string connectionString, string dbType = "mysql")
        {
            _defaultConnectionString = connectionString;
            _dbType = dbType;
        }

        /// <summary>
        /// 获取默认数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetDefaultConnectionString()
        {
            if (!string.IsNullOrEmpty(_defaultConnectionString))
                return _defaultConnectionString;

            return Kevin.Common.Helper.ConfigHelper.GetValue("ConnectionStrings:dbConnection");
        }

        /// <summary>
        /// 根据连接字符串创建数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>DbConnection</returns>
        public DbConnection CreateDbConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (_dbType == "mysql")
            {
                return new MySqlConnection(connectionString);
            }
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// 创建默认数据库连接
        /// </summary>
        /// <returns>DbConnection</returns>
        public DbConnection CreateDbConnection()
        {
            return CreateDbConnection(GetDefaultConnectionString());
        }

        /// <summary>
        /// 针对数据库执行自定义的sql查询，返回泛型List，可自定义数据库
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">自定义查询Sql</param>
        /// <param name="parameters">sql参数</param>
        /// <param name="connection">数据库连接</param>
        /// <remarks>connection = db.Database.GetDbConnection()</remarks>
        /// <returns></returns>
        public IList<T> SelectFromSql<T>(string sql, Dictionary<string, object> parameters = default) where T : class
        {
            using var connection = CreateDbConnection();
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 600;
                    command.CommandText = sql;

                    AddParameters(command, parameters);

                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return DataHelper.DataTableToList<T>(dataTable);
                    }
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// 针对数据库执行自定义的sql
        /// </summary>
        /// <param name="sql">自定义查询Sql</param>
        /// <param name="parameters">sql参数</param>
        /// <param name="connection">自定义数据库连接</param>
        /// <remarks>connection = db.Database.GetDbConnection()</remarks>
        /// <returns>受影响的行数</returns>
        public int ExecuteSql(string sql, Dictionary<string, object> parameters = default)
        {
            using var connection = CreateDbConnection();
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 600;
                    command.CommandText = sql;

                    AddParameters(command, parameters);

                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        } 
        /// <summary>
        /// 批量执行SQL
        /// </summary>
        /// <param name="sqlList">SQL语句列表</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">是否使用事务</param>
        public void ExecuteSqlBatch(List<string> sqlList, bool transaction = true)
        {
            DbTransaction dbTransaction = null;
            using var connection = CreateDbConnection();
            try
            {

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                if (transaction)
                    dbTransaction = connection.BeginTransaction();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 600;
                    if (dbTransaction != null)
                        command.Transaction = dbTransaction;

                    foreach (var sql in sqlList)
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }

                if (dbTransaction != null)
                    dbTransaction.Commit();
            }
            catch
            {
                if (dbTransaction != null)
                    dbTransaction.Rollback();
                throw;
            }
            finally
            {
                dbTransaction?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        } 

        /// <summary>
        /// 向命令对象添加参数
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="parameters">参数字典</param>
        private void AddParameters(DbCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return;

            foreach (var item in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = item.Key;
                parameter.Value = item.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
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