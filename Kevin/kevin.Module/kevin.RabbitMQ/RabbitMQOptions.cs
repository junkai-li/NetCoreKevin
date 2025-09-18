using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ
{
    public class RabbitMQOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string? HostName { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        ///  密码
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string? VirtualHost { get; set; }
    }
}
