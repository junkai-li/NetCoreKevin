using Kevin.Asr.AliCloud;
using Kevin.Asr.AliCloud.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Asr
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAliCloudAsrService(this IServiceCollection services, Action<AliAsrSetting> action)
        {
            services.Configure(action); 
            services.AddSingleton<IAsrService, AliCloudAsrService>();
        }
    }
}
