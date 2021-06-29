using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Common.Json
{
    public class JsonHelper
    {


        /// <summary>
        /// 通过 Key 获取 Value
        /// </summary>
        /// <returns></returns>
        public static string GetValueByKey(string json, string key)
        {
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);

                return jo[key].ToString();
            }
            catch
            {
                throw new Exception(json);
            }
        }




        /// <summary>
        /// DataRow转JSON
        /// </summary>
        /// <param name="row">DataRow</param>
        /// <returns>JSON格式对象</returns>
        public static object DataRowToJSON(DataRow row)
        {
            Dictionary<string, object> dataList = new Dictionary<string, object>();
            foreach (DataColumn column in row.Table.Columns)
            {
                dataList.Add(column.ColumnName, row[column]);
            }

            return ObjectToJSON(dataList);
        }




        /// <summary>
        /// DataRow转对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="row">DataRow</param>
        /// <returns>JSON格式对象</returns>
        public static T DataRowToObject<T>(DataRow row)
        {
            return JSONToObject<T>(DataRowToJSON(row).ToString());
        }




        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>JSON格式对象</returns>
        public static List<T> DataTableToList<T>(DataTable table)
        {
            return JSONToList<T>(ObjectToJSON(table));
        }




        /// <summary>
        /// Json转List
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>JSON格式对象</returns>
        public static List<T> JSONToList<T>(string jsonText)
        {
            return JSONToObject<List<T>>(jsonText);
        }




        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj)
        {
            try
            {
                JsonSerializerSettings jset = new JsonSerializerSettings();
                jset.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jset.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy'/'MM'/'dd' 'HH':'mm':'ss" });

                //设置属性名为首字母小写得驼峰形式
                jset.ContractResolver = new CamelCasePropertyNamesContractResolver();

                return JsonConvert.SerializeObject(obj, jset).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
            }
        }




        /// <summary> 
        /// JSON文本转对象
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JSONToObject<T>(string jsonText)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonText.Replace("undefined", "null"));
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
            }
        }



        /// <summary>
        /// 没有Key的 Json 转 数组List
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static List<JToken> JsonToArrayList(string strJson)
        {
            return ((JArray)JsonConvert.DeserializeObject(strJson)).ToList();
        }
    }
}
