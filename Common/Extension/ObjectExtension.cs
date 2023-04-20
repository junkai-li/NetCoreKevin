using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{

    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
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
}
