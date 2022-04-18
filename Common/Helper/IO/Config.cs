using Microsoft.Extensions.Configuration;
using System;

namespace Common.IO
{

    /// <summary>
    /// 配置文件操作类
    /// </summary>
    public class Config
    {

        /// <summary>
        /// 读取项目配置文件(appsettings.json)
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot Get()
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
            return configuration;
        }


    }
}
