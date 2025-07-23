using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace kevin_ESL
{
    public static class ServiceCollectionExtensions
    { 
        public static void AddESLClient(this IServiceCollection services)
        {  
            services.AddTransient<IESLClient, ESLClient>();
        }
    }
}
