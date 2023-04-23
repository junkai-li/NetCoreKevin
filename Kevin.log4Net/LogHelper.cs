using log4net;
using log4net.Appender;
using log4net.Layout;
using ILog = log4net.ILog;

namespace Kevin.log4Net
{
    public class LogHelper
    {
        public static ILog logger;
        static LogHelper()
        {
            logger = LogManager.GetLogger(typeof(LogHelper));
        }

        private static object lockObj = new object();
        private static Dictionary<string, ILog> diction = new Dictionary<string, ILog>();

        public static ILog GetLog(string logName)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            var logKey = logName + date;
            if (!diction.ContainsKey(logKey))
            {
                lock (lockObj)
                {

                    if (!diction.ContainsKey(logKey))
                    {
                        var repositoryName = "rep" + logKey;
                        var repository = LogManager.CreateRepository(repositoryName);
                        repository.ResetConfiguration();

                        //根据名称生成ILog对象
                        RollingFileAppender append = new RollingFileAppender();
                        append.Name = logKey;
                        append.File = $"Log/{date}/{logName}.log";
                        append.AppendToFile = true;
                        append.MaxSizeRollBackups = 100;
                        append.MaximumFileSize = "5MB";
                        append.StaticLogFileName = false;
                        append.LockingModel = new FileAppender.MinimalLock();
                        append.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                        string pattern = "记录时间：%date  线程ID:[%thread] %n日志级别：%-5level%n记录位置：%location%n 消息：%message%newline%n------------------------------------------%n";
                        PatternLayout layout = new PatternLayout(pattern);
                        append.Layout = layout;
                        append.ActivateOptions();
                        log4net.Config.BasicConfigurator.Configure(repository, append);

                        diction.Add(logKey, LogManager.GetLogger(repositoryName, logKey));
                    }

                }
            }
            return diction[logKey];
        }

        public static ILog GetLog(Type type)
        {
            return LogManager.GetLogger(type);
        }

    }

    public class LogHelper<T>
    {
        public static ILog logger;
        static LogHelper()
        {
            logger = LogManager.GetLogger(typeof(T));
        }


    }
}
