using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Extension.Autofac
{
    /// <summary>
    /// 标记自动注入特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class AutowiredAttribute : Attribute, IServiceProviderFactory<IServiceProvider>
    {
        IServiceProvider IServiceProviderFactory<IServiceProvider>.CreateBuilder(IServiceCollection services) => throw new NotImplementedException();
        IServiceProvider IServiceProviderFactory<IServiceProvider>.CreateServiceProvider(IServiceProvider containerBuilder) => containerBuilder;
    }
}
