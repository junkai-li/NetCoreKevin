using kevin.Ioc.IocAttrributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.Ioc
{
    /// <summary>
    /// 自定义的控制器创建对象，以便使用ioc创建控制器，其实IOC就是一个字典Dictionary
    /// </summary>
    public class IocServiceBaseControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext context)
        {
            var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
            //IOC容器完成实例化
            var controllerInstance = context.HttpContext.RequestServices.GetRequiredService(controllerType);
            //Controller控制器属性注入，IocPropertyAttribute标记需要的属性，不是所有的属性
            foreach (var prop in controllerType.GetProperties().Where(prop => prop.IsDefined(typeof(IocPropertyAttribute), true)))
            {
                try
                {
                    var propValue = context.HttpContext.RequestServices.GetRequiredService(prop.PropertyType);
                    if (propValue != null)
                    {
                        prop.SetValue(controllerInstance, propValue);
                    }
                }
                catch
                {
                    Console.WriteLine(prop.PropertyType.ToString()+ "IocProperty注入失败");
                }


            }

            //Controller控制器方法注入，IocMethodAttribute标记需要的方法，不是所有的方法
            foreach (var method in controllerType.GetMethods().Where(method => method.IsDefined(typeof(IocMethodAttribute), true)))
            {
                try
                {
                    var methodParameters = method.GetParameters();
                    List<object> listMethodParameters = new List<object>();
                    foreach (var para in methodParameters)
                    {
                        var paraValue = context.HttpContext.RequestServices.GetRequiredService(para.ParameterType);
                        listMethodParameters.Add(paraValue);
                    }

                    //调用Controller控制器方法
                    method.Invoke(controllerInstance, listMethodParameters.ToArray());
                }
                catch
                {
                    Console.WriteLine("IocMethod注入失败");
                } 
            }
            return controllerInstance;
        }

        public void Release(ControllerContext context, object controller)
        {
            //throw new NotImplementedException(); 
        }
    }
}
