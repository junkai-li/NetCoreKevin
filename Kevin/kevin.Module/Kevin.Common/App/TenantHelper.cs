using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.App
{
    public class TenantHelper
    {
        /// <summary>
        /// GetSettingsTenantId
        /// </summary>
        /// <returns></returns>
        public static string GetSettingsTenantId()
        {
            try
            { 
                var ev = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
                if (string.IsNullOrEmpty(ev))
                {
                    ev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                }
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                if (!string.IsNullOrEmpty(ev))
                {
                    builder = new ConfigurationBuilder().AddJsonFile("appsettings." + ev + ".json");
                }
                IConfigurationRoot configuration = builder.Build();
                return configuration["TenantId"];
            }
            catch (Exception)
            {
                return "1000";
            }
        }
    }
}
