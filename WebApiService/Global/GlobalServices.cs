using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiService.Global
{
    /// <summary>
    /// Ioc Helper, for quick initialize service at any where.
    /// 容器帮助类，为方便在任何地方轻松调用容器中的服务
    /// </summary>
    public static class GlobalServices
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
