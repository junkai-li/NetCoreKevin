using Microsoft.Extensions.DependencyInjection;

namespace kevin.Ioc.TieredServiceRegistration
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
