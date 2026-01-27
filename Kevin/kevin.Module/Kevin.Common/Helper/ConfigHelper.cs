using Microsoft.Extensions.Configuration;
using System;

namespace Kevin.Common.Helper
{
    public static class ConfigHelper
    {
        private static IConfiguration _configuration;

        /// <summary>
        /// 初始化配置（通常在Program.cs中调用一次）
        /// </summary>
        /// <param name="config">配置实例</param>
        public static void Initialize(IConfiguration config)
        {
            _configuration = config;
        }

        /// <summary>
        /// 泛型方法读取配置节
        /// </summary>
        /// <typeparam name="T">配置类类型</typeparam>
        /// <param name="sectionKey">配置节键名</param>
        /// <returns>配置实例</returns>
        public static T GetSection<T>(string sectionKey) where T : class, new()
        {
            if (_configuration == null)
                throw new InvalidOperationException("配置未初始化，请先调用Initialize方法");

            return _configuration.GetSection(sectionKey).Get<T>();
        }

        /// <summary>
        /// 直接读取配置值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>配置值</returns>
        public static string GetValue(string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("配置未初始化，请先调用Initialize方法");

            return _configuration?[key];
        } 
    }
}
