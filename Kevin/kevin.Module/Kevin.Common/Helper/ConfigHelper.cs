using Microsoft.Extensions.Configuration;
using System;

namespace Kevin.Common.Helper
{
    public static class ConfigHelper
    {
        public static IConfiguration Configuration;

        /// <summary>
        /// 初始化配置（通常在Program.cs中调用一次）
        /// </summary>
        /// <param name="config">配置实例</param>
        public static void Initialize(IConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// 泛型方法读取配置节
        /// </summary>
        /// <typeparam name="T">配置类类型</typeparam>
        /// <param name="sectionKey">配置节键名</param>
        /// <returns>配置实例</returns>
        public static T GetSection<T>(string sectionKey) where T : class, new()
        {
            if (Configuration == null)
                throw new InvalidOperationException("配置未初始化，请先调用Initialize方法");

            return Configuration.GetSection(sectionKey).Get<T>();
        }

        /// <summary>
        /// 泛型方法读取配置节
        /// </summary>
        /// <typeparam name="T">配置类类型</typeparam>
        /// <param name="sectionKey">配置节键名</param>
        /// <returns>配置实例</returns>
        public static IConfigurationSection GetSection(string sectionKey)
        {
            if (Configuration == null)
                throw new InvalidOperationException("配置未初始化，请先调用Initialize方法");

            return Configuration.GetSection(sectionKey);
        }

        /// <summary>
        /// 直接读取配置值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>配置值</returns>
        public static string GetValue(string key)
        {
            if (Configuration == null)
                throw new InvalidOperationException("配置未初始化，请先调用Initialize方法");

            return Configuration?[key];
        }
    }
}
