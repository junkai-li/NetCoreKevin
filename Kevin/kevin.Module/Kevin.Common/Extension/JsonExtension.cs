using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kevin.Common.Extension
{
    /// <summary>
    /// 拓展类
    /// </summary>
    public static class JsonExtension
    {
        static JsonExtension()
        {
            JsonConvert.DefaultSettings = () => DefaultJsonSetting;
        }
        public static JsonSerializerSettings DefaultJsonSetting = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff"
        };

        /// <summary>
        /// 通过 Key 获取 Value
        /// </summary>
        /// <returns></returns>
        public static string GetValueByKey(this string json, string key)
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
        /// 通过 Key 获取 Value
        /// </summary>
        /// <returns></returns>
        public static string GetValueByKeyNoTrr(this string json, string key)
        {
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                if (jo.ContainsKey(key))
                {
                    return jo[key].ToString();
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
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

            return ToJson(dataList);
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
        public static List<T> DataTableToList<T>(this DataTable table)
        {
            return JSONToList<T>(ToJson(table));
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

        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj).ToString();
        }
        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj, JsonSerializerSettings jsonSerializer)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializer).ToString();
        }
        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object ToObject(this string jsonStr, Type type)
        {
            return JsonConvert.DeserializeObject(jsonStr, type);
        }
        /// <summary>
        /// 异步序列化Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> SerializeJsonAsync<T>(this T obj)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, //转换成驼峰写法
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //避免中文被转义
            };
            await using var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, options);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();
            return result;
        }
        /// <summary>
        /// 异步反序列化(关于异常: 请确保你的序列化格式正确 2.确保Json大小写 是否对上属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static async ValueTask<T> DeserializJsonAync<T>(this string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,//不区分大小写
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //避免中文被转义
            };
            var bytes = Encoding.Default.GetBytes(jsonString);
            await using var stream = new MemoryStream(bytes);
            var result = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, options);
            return result;
        }
    }
}
