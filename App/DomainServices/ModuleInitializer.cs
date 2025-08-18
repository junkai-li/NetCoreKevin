using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Domain
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            Console.WriteLine("DomainServices-ModuleInitializer");
        }
    }
}
