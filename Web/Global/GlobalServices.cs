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
    }
}
