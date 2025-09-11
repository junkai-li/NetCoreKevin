using AutoMapper;
using Kevin.Common.Extension;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{

    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static TDestination MapTo<TDestination>(this object obj)
        {
            return obj.Adapt<TDestination>();
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// There must be a mapping between objects before calling this method.
        /// 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam><typeparam name="TDestination">Destination type</typeparam><param name="source">Source object</param><param name="destination">Destination object</param>
        /// <returns/>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) where TDestination : class, new()
        {
            if (IsEnumerableType(typeof(TSource)))
            {
                throw new ArgumentException("This method can't support class which inherit IEnumberable`1");
            }
            if (destination == null) destination = new TDestination();
            MapToSimpleObject(source, destination);
            return destination;
        }
        /// <summary>
        /// 单条实体类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TSource">Dto类型</typeparam>
        /// <typeparam name="TDestination">要被转化的数据</typeparam>
        /// <param name="source">转化之后的实体</param>
        /// <returns></returns>
        public static TDestination MapTo<TDestination>(this object source, Action<IMapperConfigurationExpression> configure)
            where TDestination : class, new()
        {
            if (source == null) return new TDestination();
            var config = new MapperConfiguration(configure);
            var mapper = config.CreateMapper();
            return mapper.Map<TDestination>(source);
        }
        /// <summary>
        /// 单条实体类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TSource">转化之后的实体</typeparam>
        /// <typeparam name="TDestination">要被转化的数据</typeparam>
        /// <param name="configure">自定义类型转类型</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, Action<IMapperConfigurationExpression> configure)
        where TDestination : class, new()
        where TSource : class
        {
            List<Action<IMapperConfigurationExpression>> actions = new List<Action<IMapperConfigurationExpression>>();
            Action<IMapperConfigurationExpression> action = (cfg) =>
            {
                cfg.CreateMap<TSource, TDestination>();
            };
            actions.Add(action);
            if (configure != null) actions.Add(configure);
            if (source == null) return new TDestination();
            var config = new MapperConfiguration(cfg =>
            {
                actions.ForEach(a => a.Invoke(cfg));
            });
            var mapper = config.CreateMapper();
            return mapper.Map<TDestination>(source);
        }
        /// <summary>
        /// 实体列表类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TDestination">Dto类型</typeparam>
        /// <typeparam name="TSource">要被转化的数据</typeparam>
        /// <param name="source">可以使用这个扩展方法的类型，任何引用类型</param>
        /// <returns>转化之后的实体列表</returns>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
            var mapper = config.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 实体列表类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TDestination">Dto类型</typeparam>
        /// <typeparam name="TSource">要被转化的数据</typeparam>
        /// <param name="source">可以使用这个扩展方法的类型，任何引用类型</param>
        /// <returns>转化之后的实体列表</returns>
        public static List<TDestination> MapToList<TDestination>(this object source, Action<IMapperConfigurationExpression> configure)
            where TDestination : class
        {
            if (source == null) return new List<TDestination>();
            var config = new MapperConfiguration(configure);
            var mapper = config.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// Does class implement IEnumerable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsEnumerableType(Type type)
        {
            var hasInterface = type.GetInterface("IEnumerable`1");
            return hasInterface != null;
        }

        /// <summary>
        /// Map to generic type which inherit IEnumerable
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static void MapToGenericEnumerable<TSource, TDestination>(this TSource source, TDestination destination)
            where TDestination : class, new()
        {
            var sourceType = source.GetType();
            var destType = destination.GetType();
            var sourceHasInterface = sourceType.GetInterface("IEnumerable`1");
            var targetHasInterface = destType.GetInterface("IEnumerable`1");
            if (sourceHasInterface != null && targetHasInterface != null && sourceType.IsGenericType)
            {
                int count = Convert.ToInt32(sourceType.GetProperty("Count").GetValue(source, null));
                for (int i = 0; i < count; i++)
                {
                    var itemValue = sourceType.GetProperty("Item").GetValue(source, new object[] { i });
                    var destGenericType = destType.GenericTypeArguments[0];
                    var destInstance = destGenericType.Assembly.CreateInstance(destGenericType.FullName);
                    MapToSimpleObject(itemValue, destInstance);
                    var methodAdd = destType.GetMethod("Add");
                    methodAdd.Invoke(destination, new object[] { destInstance });
                }
            }
        }
        /// <summary>
        /// Map to simple object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void MapToSimpleObject<TSource, TDestination>(this TSource source, TDestination destination)
            where TDestination : class, new()
        {
            var sourceType = source.GetType();
            var sourceProps = sourceType.GetProperties();
            var targetType = destination.GetType();
            var targetProps = targetType.GetProperties();
            foreach (var p in sourceProps)
            {
                var targetValueType = targetProps.FirstOrDefault(r => r.Name == p.Name);
                if (targetValueType != null)
                {
                    var valueType = sourceType.GetProperty(p.Name);
                    var modelType = targetType.GetProperty(p.Name);
                    if (valueType.PropertyType != modelType.PropertyType)
                        continue;  

                    var value = valueType.GetValue(source);
                    if (value == null)
                        continue; 

                    var preSetValue = value; 
                    //only support simple object type not support collection
                    if (!valueType.PropertyType.IsValueType && valueType.PropertyType != typeof(String))
                    {
                        var targetPropType = targetValueType.PropertyType;
                        var targetPropInstance = targetPropType.Assembly.CreateInstance(targetPropType.FullName);
                        if (IsEnumerableType(source.GetType()))
                        {
                            MapToGenericEnumerable(source, targetPropInstance);
                        }
                        else
                        {
                            MapToSimpleObject(source, targetPropInstance);
                        }
                        preSetValue = targetPropInstance;
                    } 
                    targetValueType.SetValue(destination, preSetValue);
                }
            }
        }
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.Type)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// 判断字符是否在数组中存在
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool In<T>(this T s, params T[] arr)
        {
            return arr != null && arr.Contains(s);
        }
        /// <summary>
        /// 判断字符是否在数组中存在
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool In<T>(this T s, IEnumerable<T> arr)
        {
            return arr != null && arr.Contains(s);
        }
        /// <summary>
        /// 判断字符是否在数组中存在
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T s, params T[] arr)
        {
            return arr == null || !arr.Contains(s);
        }
        /// <summary>
        /// 当前项 不存在于集合之中
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T s, IEnumerable<T> arr)
        {
            return arr == null || !arr.Contains(s);
        }

        /// <summary>
        /// 判断是否为null，null或0长度都返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this T value)
            where T : class
        {
            #region 1.对象级别

            //引用为null
            bool isObjectNull = value is null;
            if (isObjectNull)
            {
                return true;
            }

            //判断是否为集合
            var tempEnumerator = (value as IEnumerable)?.GetEnumerator();
            if (tempEnumerator == null) return false;//这里出去代表是对象 且 引用不为null.所以为false

            #endregion 1.对象级别

            #region 2.集合级别

            //到这里就代表是集合且引用不为空，判断长度
            //MoveNext方法返回tue代表集合中至少有一个数据,返回false就代表0长度
            bool isZeroLenth = tempEnumerator.MoveNext() == false;
            if (isZeroLenth) return true;

            return isZeroLenth;

            #endregion 2.集合级别
        }

        /// <summary>
        /// 序列化为Json串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToJson(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var str = JsonConvert.SerializeObject(obj);
                    return str;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }

    public static partial class Extention
    {
        private static BindingFlags _bindingFlags { get; }
            = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 实体类转json数据，速度快
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        public static string EntityToJson(this object t)
        {
            if (t == null)
                return null;
            string jsonStr = "";
            jsonStr += "{";
            PropertyInfo[] infos = t.GetType().GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                jsonStr = jsonStr + "\"" + infos[i].Name + "\":\"" + infos[i].GetValue(t).ToString() + "\"";
                if (i != infos.Length - 1)
                    jsonStr += ",";
            }
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            return obj.ToJson().ToObject<T>();
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="rootNodeName">根节点名(建议设为xml)</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj, string rootNodeName)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr, rootNodeName);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 是否拥有某属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static bool ContainsProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags) != null;
        }

        /// <summary>
        /// 获取某属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags);
        }

        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj);
        }

        /// <summary>
        /// 获取某属性值 报错返回空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValueTryEny(this object obj, string propertyName)
        {
            try
            {
                return obj.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj);
            }
            catch
            {

                return "";
            }

        }
        /// <summary>
        /// 设置某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static object SetPropertyValue(this object obj, string propertyName, object value)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, _bindingFlags);
            if (propertyInfo.CanWrite) propertyInfo.SetValue(obj, value);
            return obj;
        }

        /// <summary>
        /// 是否拥有某字段
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static bool ContainsField(this object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName, _bindingFlags) != null;
        }

        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static object GetGetFieldValue(this object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName, _bindingFlags).GetValue(obj);
        }

        /// <summary>
        /// 设置某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetFieldValue(this object obj, string fieldName, object value)
        {
            obj.GetType().GetField(fieldName, _bindingFlags).SetValue(obj, value);
        }

        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(this object obj, string methodName)
        {
            return obj.GetType().GetMethod(methodName, _bindingFlags);
        }

        /// <summary>
        /// 改变实体类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeType(this object obj, Type targetType)
        {
            return obj.ToJson().ToObject(targetType);
        }

        /// <summary>
        /// 改变实体类型
        /// </summary>
        /// <typeparam name="T">目标泛型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T ChangeType<T>(this object obj)
        {
            return obj.ToJson().ToObject<T>();
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="obj">原对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeType_ByConvert(this object obj, Type targetType)
        { 
                if (obj.GetType().ToString() == "MySql.Data.Types.MySqlDateTime") obj = obj.ToDateTime();//MySql时间类型转成C# DateTime类型
                if ((obj.GetType() == typeof(Byte) || obj.GetType() == typeof(SByte)))
                {
                    obj = obj.ToTryInt16();
                }
                object resObj;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    NullableConverter newNullableConverter = new NullableConverter(targetType);
                    resObj = newNullableConverter.ConvertFrom(obj);
                }
                else
                {
                    resObj = Convert.ChangeType(obj, targetType);
                } 
                return resObj; 
        }
        public static bool ToBoolean(this object value)
        {
            try
            {
                bool obj = Convert.ToBoolean(value);
                return obj;
            }
            catch
            {
                return false;
            }
        }
        public static DateTime ToDateTime(this object value)
        {
            if (value == null) return default;
            DateTime data = default;
            DateTime.TryParse(value.ToString(), out data);
            return data;
        }
        public static string ToTimeFormat1(this object value)
        {
            try
            {
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return "";
            }
        }
        public static byte[] ToByte(this object value)
        {
            try
            {
                return (value == DBNull.Value) ? new byte[0] : (byte[])value;
            }
            catch
            {
                return new byte[0];
            }
        }
        public static sbyte ToSByte(this object value)
        {
            try
            {
                return sbyte.Parse(value.ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static byte[] FromBase64String(this string value)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                return new byte[0];
            }
        }
        public static short ToInt16(this object value)
        {
            try
            {
                if (value == null) return 0;
                short obj = Convert.ToInt16(value);
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        public static int ToInt32(this object value)
        {
            try
            {
                if (value == null) return 0;
                int obj = Convert.ToInt32(value);
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        public static long ToInt64(this object value)
        {
            try
            {
                if (value == null) return 0;
                long obj = Convert.ToInt64(value);
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        public static double ToDouble(this object value)
        {
            try
            {
                double obj = Convert.ToDouble(value);
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 转换成float类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToSingle(this object value)
        {
            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return 0;
            }
        }
        public static decimal ToDecimal(this object value)
        {
            try
            {
                decimal obj = Convert.ToDecimal(value);
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        public static string ToEString(this object value)
        {
            try
            {
                if (value == null) return "";
                string obj = Convert.ToString(value);
                return obj;
            }
            catch
            {
                return "";
            }
        }

        public static decimal ToDecimalEx(this object value, int point = 2)
        {
            try
            {
                return Math.Round(value.ToDouble(), point, MidpointRounding.AwayFromZero).ToDecimal();
            }
            catch
            {
                return 0;
            }
        }
         

        public static short ToTryInt16(this object value)
        {
            if (value == null) return 0;
            Int16 data = 0;
            Int16.TryParse(value.ToString(), out data);
            return data;
        }
        public static int ToTryInt32(this object value)
        {
            if (value == null) return 0;
            Int32 data = 0;
            Int32.TryParse(value.ToString(), out data);
            return data;
        }
        public static long ToTryInt64(this object value)
        {
            if (value == null) return 0;
            Int64 data = 0;
            Int64.TryParse(value.ToString(), out data);
            return data;
        }
        public static double ToTryDouble(this object value)
        {
            if (value == null) return 0;
            double data = 0;
            double.TryParse(value.ToString(), out data);
            return data;
        }
        /// <summary>
        /// 转换成float类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToTrySingle(this object value)
        {
            if (value == null) return 0;
            float data = 0;
            float.TryParse(value.ToString(), out data);
            return data;
        }
        public static decimal ToTryDecimal(this object value)
        {
            if (value == null) return 0;
            decimal data = 0;
            Decimal.TryParse(value.ToString(), out data);
            return data;
        }

        public static DateTime ToTryDateTime(this object value)
        {
            if (value == null) return DateTime.MinValue;
            DateTime data = DateTime.MinValue;
            DateTime.TryParse(value.ToString(), out data);
            return data;
        }

        public static sbyte ToTrySByte(this object value)
        {
            if (value == null) return 0;
            sbyte data = 0;
            SByte.TryParse(value.ToString(), out data);
            return data;

        }
    }
}
