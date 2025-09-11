using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Consul.Models
{
    public class ConsulSetting
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceIP"></param>
        /// <param name="serviceHealthCheck"></param>
        /// <param name="consulAddress"></param>
        public ConsulSetting(string serviceName, string serviceIP, string serviceHealthCheck, string consulAddress, string servicePort)
        {
            this.ServiceName = serviceName;
            this.ServiceIP = serviceIP;
            this.ServiceHealthCheck = serviceHealthCheck;
            this.ConsulAddress = consulAddress;
            this.ServicePort = servicePort; 
        }
        public ConsulSetting()
        { 
        }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; init; } = "";

        /// <summary>
        ////服务IP
        /// </summary>
        public string ServiceIP { get; init; } = "";

        /// <summary>
        ///健康检查地址
        /// </summary>
        public string ServiceHealthCheck { get; init; } = "";
        /// <summary>
        /// //服务端口 因为要运行多个实例，端口不能在appsettings.json里配置，在docker容器运行时传入
        /// </summary>
        public string ServicePort { get; init; } = "";
        /// <summary>   
        /// consul地址
        /// </summary>
        public string ConsulAddress { get; init; } = "";
    }
}
