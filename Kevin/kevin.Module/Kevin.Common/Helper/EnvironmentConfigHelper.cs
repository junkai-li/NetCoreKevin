using System;
using System.Collections.Generic;

namespace Kevin.Common.Helper
{
    public class EnvironmentConfigHelper
    {
        /// <summary>
        /// 开发环境
        /// </summary>
        public const string Development = "Development";
        /// <summary>
        /// 测试环境
        /// </summary>
        public const string Test = "Test";
        /// <summary>
        /// 正式环境
        /// </summary>
        public const string Formal = "";

        public static Dictionary<string, string> EnvironmentDictionary = new Dictionary<string, string>
        {
            { Development, "开发环境---配置appsettings.Development.json" },
            { Test, "测试环境---配置appsettings.Test.json" },
            { Formal, "正式环境---配置appsettings.json" }
        };

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



        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="env"></param>
        public static void SetEnvironment(string env)
        {
            //为了兼容性，如果传入的环境变量为null，则默认为正式环境
            if (env == null)
            {
                env = Formal;
            }
            ConsoleHelper.PrintFrameworkName("当前环境变量:" + EnvironmentDictionary[env]);
            Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", env);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);
        }
    }
}
