using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.log4Net
{
    public static class EnvironmentConfigHelper
    {

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <returns></returns>
        public static string GetEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(env))
            {
                env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }
            return env;
        }
    }
}
