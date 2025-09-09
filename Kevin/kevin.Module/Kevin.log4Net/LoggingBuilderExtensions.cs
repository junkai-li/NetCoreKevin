using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.log4Net
{
    public static class LoggingBuilderExtensions
    {
        public static void UseKevinLog4Net(this ILoggingBuilder log,string log4NetConfigFile= ".\\Configs\\_\\log4.config")
        { 
            log.AddLog4Net(log4NetConfigFile);
            var log1 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log1.Info("Hello, UseKevinLog4Net!"); 
        }
    }
}
