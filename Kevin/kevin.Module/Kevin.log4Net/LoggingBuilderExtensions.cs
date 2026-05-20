using log4net;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Kevin.log4Net
{
    public static class LoggingBuilderExtensions
    {
        public static void UseKevinLog4Net(this ILoggingBuilder log, string log4NetConfigFile = "")
        {
            // 默认相对路径（平台无关）
            var relativeDefault = Path.Combine("LogConfigs", $"log4.{EnvironmentConfigHelper.GetEnvironment()}.config");

            // 采用传入路径或默认路径
            var relativePath = string.IsNullOrWhiteSpace(log4NetConfigFile) ? relativeDefault : log4NetConfigFile;

            // 规范化路径分隔符
            relativePath = relativePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            // 如果是相对路径，将其解析到应用运行目录
            string configPath = Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));

            var logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

            if (!File.Exists(configPath))
            {
                // 在 Linux/容器中常见：config 未复制到输出目录或路径写法问题
                logger.Warn($"log4net config file not found: {configPath}. Skipping AddLog4Net.");
                Console.WriteLine($"log4net config file not found: {configPath}. Skipping AddLog4Net.");
                return;
            }

            // 调用扩展并记录
            log.AddLog4Net(configPath);
            logger.Info("Hello, UseKevinLog4Net!");
        }
    }
}
