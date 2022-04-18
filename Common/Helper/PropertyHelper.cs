using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class PropertyHelper
    {

        /// <summary>  
        /// 反射得到实体类的字段名称和值  
        /// </summary>  
        /// <typeparam name="T">实体类</typeparam>  
        /// <param name="t">实例化</param>  
        /// <returns></returns>  
        public static Dictionary<object, object> GetProperties<T>(T t)
        {
            var ret = new Dictionary<object, object>();
            if (t == null) { return null; }
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0) { return null; }
            foreach (PropertyInfo item in properties)
            {

                string name = item.Name;
                object value = item.GetValue(t, null);

                if (item.PropertyType == typeof(DateTime))
                {
                    ret.Add(name, Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss.fff"));
                }
                else if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    ret.Add(name, value);
                }
            }
            return ret;
        }



        /// <summary>  
        /// 反射得到实体类的字段显示名称和值  
        /// </summary>  
        /// <typeparam name="T">实体类</typeparam>  
        /// <param name="t">实例化</param>  
        /// <returns></returns>  
        public static Dictionary<object, object> GetPropertiesDisplayName<T>(T t)
        {
            var ret = new Dictionary<object, object>();
            if (t == null) { return null; }
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0) { return null; }
            foreach (PropertyInfo item in properties)
            {
                var displayName = item.CustomAttributes.Where(t => t.AttributeType.Name == "DisplayNameAttribute").Select(t => t.ConstructorArguments.Select(v => v.Value).FirstOrDefault()).FirstOrDefault();

                string name = displayName != null ? displayName.ToString() : item.Name;
                object value = item.GetValue(t, null);

                if (item.PropertyType == typeof(DateTime))
                {
                    ret.Add(name, Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss.fff"));
                }
                else if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    ret.Add(name, value);
                }
            }
            return ret;
        }


        /// <summary>
        /// 给对象赋值的方法(不赋地址)(同一个类型)
        /// </summary>
        /// <param name="left">=号左边</param>
        /// <param name="right">=号右边</param>
        public static void Assignment<T>(T left, T right)
        {
            Type type = left.GetType();
            List<PropertyInfo> pList = type.GetProperties().ToList();
            for (int i = 0; i < pList.Count; i++)
            {
                //根据属性名获得指定的属性对象
                PropertyInfo gc = type.GetProperty(pList[i].Name);


                //验证属性是否可以Set
                if (gc.CanWrite == true)
                {
                    //设置属性的值
                    gc.SetValue(left, pList[i].GetValue(right, null), null);
                }
            }
        }


        /// <summary>
        /// 给对象赋值的方法(不赋地址)(不同类型)
        /// </summary>
        /// <param name="left">=号左边</param>
        /// <param name="right">=号右边</param>
        public static void Assignment<L, R>(L left, R right)
        {
            var ltype = left.GetType();

            List<PropertyInfo> lList = ltype.GetProperties().ToList();

            List<PropertyInfo> rList = right.GetType().GetProperties().ToList();

            for (int i = 0; i < lList.Count; i++)
            {
                //根据属性名获得指定的属性对象
                PropertyInfo gc = ltype.GetProperty(lList[i].Name);


                //验证属性是否可以Set
                if (gc.CanWrite == true)
                {
                    try
                    {
                        var value = rList.Where(t => t.Name == gc.Name).FirstOrDefault().GetValue(right, null);

                        //设置属性的值
                        gc.SetValue(left, value, null);
                    }
                    catch
                    {

                    }

                }
            }
        }


        /// <summary>
        /// 将一组List赋值到另一组List(不同类型)
        /// </summary>
        /// <param name="lift"></param>
        /// <param name="right"></param>
        public static List<L> Assignment<L, R>(List<R> right) where L : new()
        {
            var lift = new List<L>();

            foreach (var r in right)
            {
                var l = new L();

                Assignment<L, R>(l, r);

                lift.Add(l);
            }

            return lift;
        }





        /// <summary>
        /// 比较两个实体的值输出差异结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">原始值</param>
        /// <param name="after">修改后的值</param>
        /// <returns></returns>
        public static string ComparisonEntity<T>(T original, T after) where T : new()
        {
            var retValue = "";

            var fields = typeof(T).GetProperties();

            for (int i = 0; i < fields.Length; i++)
            {
                var pi = fields[i];

                string oldValue = pi.GetValue(original)?.ToString();
                string newValue = pi.GetValue(after)?.ToString();

                string typename = pi.PropertyType.FullName;

                if ((typename != "System.Decimal" && oldValue != newValue) || (typename == "System.Decimal" && decimal.Parse(oldValue) != decimal.Parse(newValue)))
                {

                    retValue += DBHelper.GetEntityComment<T>(pi.Name) + ":";

                    if (typename == "System.Boolean")
                    {
                        retValue += (bool.Parse(oldValue) ? "是" : "否") + " -> ";
                        retValue += (bool.Parse(newValue) ? "是" : "否") + "； \n";
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
    }
}
