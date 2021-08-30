using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Castle.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module = Autofac.Module;

namespace Web.Extension.Autofac
{
    public class ConfigureAutofac : Module
    { 
        protected override void Load(ContainerBuilder containerBuilder)
        {
            //直接注册某一个类和接口
            //左边的是实现类，右边的As是接口
            //containerBuilder.RegisterType<TestServiceE>().As<ITestServiceE>().SingleInstance();


            #region 方法1   Load 适用于无接口注入
            //var assemblysServices = Assembly.Load("Exercise.Services");

            //containerBuilder.RegisterAssemblyTypes(assemblysServices)
            //          .AsImplementedInterfaces()
            //          .InstancePerLifetimeScope();

            //var assemblysRepository = Assembly.Load("Exercise.Repository");

            //containerBuilder.RegisterAssemblyTypes(assemblysRepository)
            //          .AsImplementedInterfaces()
            //          .InstancePerLifetimeScope();

            #endregion

            #region 方法2  选择性注入 与方法1 一样
            //            Assembly Repository = Assembly.Load("Exercise.Repository");
            //            Assembly IRepository = Assembly.Load("Exercise.IRepository");
            //            containerBuilder.RegisterAssemblyTypes(Repository, IRepository)
            //.Where(t => t.Name.EndsWith("Repository"))
            //.AsImplementedInterfaces().PropertiesAutowired();

            //            Assembly service = Assembly.Load("Exercise.Services");
            //            Assembly Iservice = Assembly.Load("Exercise.IServices");
            //            containerBuilder.RegisterAssemblyTypes(service, Iservice)
            //.Where(t => t.Name.EndsWith("Service"))
            //.AsImplementedInterfaces().PropertiesAutowired();
            #endregion

            #region 方法3  使用 LoadFile 加载服务层的程序集  将程序集生成到bin目录 实现解耦 不需要引用
            //获取项目路径
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var ServicesDllFile = Path.Combine(basePath, "Service.dll");//获取注入项目绝对路径
            var assemblysServices = Assembly.LoadFile(ServicesDllFile);//直接采用加载文件的方法
            containerBuilder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();

            var RepositoryDllFile = Path.Combine(basePath, "Service.dll");
            var RepositoryServices = Assembly.LoadFile(RepositoryDllFile);//直接采用加载文件的方法
            containerBuilder.RegisterAssemblyTypes(RepositoryServices).AsImplementedInterfaces();
            #endregion


            #region 在控制器中使用属性依赖注入，其中注入属性必须标注为public
            //在控制器中使用属性依赖注入，其中注入属性必须标注为public
//            var controllersTypesInAssembly = typeof(Startup).Assembly.GetExportedTypes()
//.Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type)).ToArray();
//            containerBuilder.RegisterTypes(controllersTypesInAssembly).PropertiesAutowired();
            #endregion
        }
    }
}
