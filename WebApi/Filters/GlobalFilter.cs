using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace WebApi.Filters
{
    public class GlobalFilter : Attribute, IActionFilter
    {

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {

        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            //格式化返回值
            resultFormatting(context.Result as ObjectResult);

            if (context.HttpContext.Response.StatusCode == 400)
            {
                string errMsg = context.HttpContext.Items["errMsg"].ToString();

                context.Result = new JsonResult(new { errMsg = errMsg });
            }
        }



        /// <summary>
        /// 返回对象格式化方法
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>处理 string null 为 "" ，List null 为 []</remarks>
        void resultFormatting(ObjectResult result)
        {

            if (result != null)
            {

                //如果返回对象整个为 null 则直接处理
                if (result.Value == null)
                {

                    //且该属性的类型为 string
                    if (result.DeclaredType.FullName == "System.String")
                    {
                        //设置该属性的值为 空
                        result.Value = string.Empty;
                    }


                    ////且该属性的类型为 List
                    if (result.DeclaredType.FullName.StartsWith("System.Collections.Generic.List"))
                    {
                        //利用反射获取 该属性的类型
                        Type t = Type.GetType(result.DeclaredType.AssemblyQualifiedName);

                        //利用该属性的类型 实例化一个值
                        var newValue = Activator.CreateInstance(t);

                        result.Value = newValue;
                    }
                }
                else
                {
                    if (result.Value.GetType().Namespace.Contains("System") == false)
                    {
                        {

                            //获取数据中全部的属性
                            var properties = result.Value.GetType().GetProperties();


                            //循环检测处理
                            foreach (var property in properties)
                            {

                                //获取该属性对应的值
                                var propertyValue = property.GetValue(result.Value);


                                //如果该属性的值为 null
                                if (propertyValue == null)
                                {

                                    //且该属性的类型为 string
                                    if (property.PropertyType.FullName == "System.String")
                                    {
                                        //设置该属性的值为 空
                                        property.SetValue(result.Value, string.Empty);
                                    }


                                    //且该属性的类型为 List
                                    if (property.PropertyType.FullName.StartsWith("System.Collections.Generic.List"))
                                    {
                                        //利用反射获取 该属性的类型
                                        Type t = Type.GetType(property.PropertyType.AssemblyQualifiedName);

                                        //利用该属性的类型 实例化一个值
                                        var newValue = Activator.CreateInstance(t);

                                        property.SetValue(result.Value, newValue);
                                    }

                                }
                                else
                                {

                                    //如果属性的值不是 null 且该属性不是 系统自带类型，则当作自定义类型进行递归操作值替换

                                    resultFormatting(new ObjectResult(propertyValue));

                                }
                            }
                        }

                    }
                }

            }

        }
    }
}
