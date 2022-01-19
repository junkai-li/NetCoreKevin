using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Global
{
    /// <summary>
    /// Ioc Helper, for quick initialize service at any where.
    /// 容器帮助类，为方便在任何地方轻松调用容器中的服务
    /// </summary>
    public static class GlobalServices
    {
        public static IServiceProvider ServiceProvider { get; set; }

        private static List<Type> _allServies = new List<Type>();
        private static List<Type> _allRepositroies = new List<Type>();
        /// <summary>
        /// All Service implementation
        /// 所有领域服务类实现类
        /// </summary>
        /// <param name="type"></param>
        public static void AddIService(Type type)
        {
            _allServies.Add(type);
        }
        /// <summary>
        /// All Repository implementation
        /// 所有仓储容器的实现类
        /// </summary>
        /// <param name="type"></param>
        public static void AddIRepositry(Type type)
        {
            _allRepositroies.Add(type);
        }
        public static Type FindService(Func<Type, bool> find)
        {
            return _allServies.FirstOrDefault(r => find(r));
        }
        public static Type FindRepository(Func<Type, bool> find)
        {
            return _allRepositroies.FirstOrDefault(r => find(r))?.GetType();
        }
        /// <summary>
        /// Set provider at the end of module registing
        /// 设置Provider用于查找容器中的服务，在模块注册的结束事件时会赋值。
        /// </summary>
        /// <param name="provider"></param>
        public static void Set(IServiceProvider provider)
        {
           
            ServiceProvider = provider;
        }

        /// <summary>
        /// 根据类型或接口获取所需的服务
        /// Get service by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Get<T>(typeof(T));
        }
        /// <summary>
        /// 显式提供获取的类型
        /// Get service by implit providing type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Get<T>(Type type = null)
        {
            var obj = (T)ServiceProvider.GetRequiredService(type);
            ServiceProvider.Autowired(obj);
            return obj;
        }
    }
}
