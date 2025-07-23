using Kevin.SMS.AliCloud;
using Kevin.SMS.TencentCloud;
using Microsoft.Extensions.DependencyInjection;

namespace Kevin.SMS
{
    public static class ServiceCollectionExtensions
    {

        public static void AddAliCloudSMS(this IServiceCollection services, Action<Kevin.SMS.AliCloud.Models.SMSSetting> action)
        {
            services.Configure(action);

            services.AddTransient<ISMS, AliCloudSMS>(); 
        }

        public static void AddTencentCloudSMS(this IServiceCollection services, Action<Kevin.SMS.TencentCloud.Models.SMSSetting> action)
        {
            services.Configure(action);

            services.AddTransient<ISMS, TencentCloudSMS>();
        }
    }
}
