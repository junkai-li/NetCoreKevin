using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.TieredServiceRegistration
{
    /// <summary>
    /// 模块初始化器
    /// </summary>
    public interface IModuleInitializer
    {
        /// <summary>
        /// 初始化服务集合
        /// </summary>
        /// <param name="services"></param>
        public void Initialize(IServiceCollection services);
    }
}
