using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;


namespace Repository.Interceptors
{
    public class SubTableInterceptor : DbCommandInterceptor
    {

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {

            var subtablesettings = System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText("subtablesettings.json")).RootElement;

            for (int y = 0; y < subtablesettings.GetArrayLength(); y++)
            {
                string table = subtablesettings[y].GetProperty("table").GetString();
                string subtype = subtablesettings[y].GetProperty("subtype").GetString();


                string sql = command.CommandText;

                if (sql.Contains("FROM [" + table + "]") && !sql.Contains("DELETE"))
                {

                    command.CommandText = "SELECT name FROM sys.objects WHERE name LIKE '" + table + "_2%'";

                    var reader = command.ExecuteReader();

                    var dataTable = new DataTable();

                    dataTable.Load(reader);

                    reader.Close();

                    var tableNameList = new List<string>();

                    for (int d = 0; d < dataTable.Rows.Count; d++)
                    {
                        tableNameList.Add(dataTable.Rows[d][0].ToString());
                    }


                    var startItem = sql.Split("OUTER APPLY (").ToList();

                    for (int i = startItem.Count - 1; i >= 0; i--)
                    {
                        int asIndexOf = startItem[i].IndexOf(") AS ");

                        string tempSql = "";

                        if (asIndexOf < 0)
                        {
                            if (sql.IndexOf(startItem[i]) != 0)
                            {
                                var startSql = sql.Substring(sql.IndexOf(startItem[i]));
                                var asSql = startSql.Split(") AS ")[1];

                                int endSqlIndexOf = startSql.IndexOf(asSql) + asSql.Length;

                                tempSql = startSql.Substring(0, endSqlIndexOf);
                            }
                            else
                            {
                                tempSql = sql;
                            }
                        }
                        else
                        {
                            tempSql = startItem[i].Substring(0, asIndexOf);
                        }

                        if (tempSql.Contains("FROM [" + table + "]"))
                        {
                            string newTempSql = "";

                            string asTable = tempSql.Substring(0, tempSql.IndexOf("]") + 1).Replace("SELECT ", "");

                            if (tempSql.Contains(asTable + ".[id] = N'"))
                            {
                                var idInfoList = tempSql.Split(".[id] = ").Where(t => t.StartsWith("N'")).ToList();

                                foreach (var idInfo in idInfoList)
                                {

                                    var idSql = idInfo.Substring(idInfo.IndexOf("N'") + 2);

                                    var idStr = idSql.Substring(0, idSql.IndexOf("'"));

                                    long id = Convert.ToInt64(idStr);

                                    var time = GetTimeById(id);

                                    var createtime = time.ToString(subtype);

                                    string newTable = table + "_" + createtime;

                                    if (tableNameList.Contains(newTable))
                                    {
                                        if (idInfoList.IndexOf(idInfo) != idInfoList.Count - 1)
                                        {
                                            newTempSql = newTempSql + tempSql.Replace("[" + table + "]", "[" + newTable + "]") + "\nUNION ALL\n";
                                        }
                                        else
                                        {
                                            newTempSql = newTempSql + tempSql.Replace("[" + table + "]", "[" + newTable + "]");
                                        }
                                    }
                                }

                                if (newTempSql == "")
                                {
                                    newTempSql = tempSql;
                                }

                            }
                            else
                            {
                                if (tableNameList.Count != 0)
                                {

                                    foreach (var newTable in tableNameList)
                                    {
                                        if (tableNameList.IndexOf(newTable) != tableNameList.Count - 1)
                                        {
                                            newTempSql = newTempSql + tempSql.Replace("[" + table + "]", "[" + newTable + "]") + "\nUNION ALL\n";
                                        }
                                        else
                                        {
                                            newTempSql = newTempSql + tempSql.Replace("[" + table + "]", "[" + newTable + "]");
                                        }
                                    }

                                }
                                else
                                {
                                    newTempSql = tempSql;
                                }
                            }

                            sql = sql.Replace(tempSql, newTempSql);

                        }
                    }

                    command.CommandText = sql;
                }



                if (sql.Contains("INSERT INTO [" + table + "]"))
                {
                    var childSql = sql.Substring(sql.IndexOf("INSERT INTO [" + table + "]"));

                    var nextInsert = childSql.Substring(11).IndexOf("INSERT INTO");

                    if (nextInsert > 0)
                    {
                        childSql = childSql.Substring(0, nextInsert);
                    }

                    var nextIf = childSql.IndexOf("IF ( (SELECT COUNT (1) FROM sys.objects WHERE name");

                    if (nextIf > 0)
                    {
                        childSql = childSql.Substring(0, nextIf);
                    }


                    var headSql = childSql.Substring(0, childSql.IndexOf("VALUES") + 6);

                    var dataSql = childSql.Replace(headSql, "").Split("\r\n").Where(t => t != "" && t != "\n").ToList();

                    sql = sql.Replace(headSql, "");

                    foreach (var data in dataSql)
                    {
                        string newData = headSql + data.Replace("),", ");");

                        string tempSql = newData.Substring(newData.IndexOf("INSERT INTO [" + table + "]"));

                        var pList = tempSql.Substring(tempSql.IndexOf("(") + 1, tempSql.IndexOf(")") - tempSql.IndexOf("(") - 1).Split(",").Select(t => t.Trim()).ToList();

                        var idIndex = pList.IndexOf("[id]");

                        var idPIndex = data.Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", "").Split("@p").Where(t => t != "").ToList();

                        var intIdPIndex = int.Parse(idPIndex[idIndex]);

                        long id = Convert.ToInt64(command.Parameters[intIdPIndex].Value);

                        var time = GetTimeById(id);

                        var createtime = time.ToString(subtype);

                        var createTable = "\nIF ( (SELECT COUNT (1) FROM sys.objects WHERE name = '" + table + "_" + createtime + "') = 0) \nBEGIN \nSELECT * INTO [" + table + "_" + createtime + "] FROM [" + table + "] \nEND";

                        newData = createTable + "\n" + newData.Replace("[" + table + "]", "[" + table + "_" + createtime + "]");

                        sql = sql.Replace(data, newData);
                    }

                    command.CommandText = sql;

                }

                if (sql.Contains("UPDATE [" + table + "]") || sql.Contains("DELETE"))
                {
                    var sqlList = sql.Split("SELECT @@ROWCOUNT;").ToList();

                    command.CommandText = "";

                    foreach (var item in sqlList)
                    {
                        if (item.Contains("@p"))
                        {
                            string tempSql = item.Replace("\n", "").Replace("\r", "").Replace(";", "");

                            var pName = tempSql.Substring(tempSql.LastIndexOf("@p"));

                            var id = Convert.ToInt64(command.Parameters[pName].Value);

                            var time = GetTimeById(id);

                            var createtime = time.ToString(subtype);

                            command.CommandText = command.CommandText + item.Replace("[" + table + "]", "[" + table + "_" + createtime + "]") + "SELECT @@ROWCOUNT;";
                        }


                    }

                }

            }

            return result;
        }



        /// <summary>
        /// 通过ID获取其中的时间戳
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DateTime GetTimeById(long id)
        {
            var idStr2 = Convert.ToString(id, 2);

            if (idStr2.Length < 63)
            {
                do
                {
                    idStr2 = "0" + idStr2;
                } while (idStr2.Length != 63);
            }

            var timeStr2 = idStr2.Substring(0, 41);

            var timeJsStamp = Convert.ToInt64(timeStr2, 2);

            var startTime = TimeZoneInfo.ConvertTimeToUtc((new DateTime(2020, 1, 1)).ToLocalTime()).ToLocalTime(); // 当地时区
            return startTime.AddMilliseconds(timeJsStamp);
        }


        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {

            return result;
        }

    }
}
