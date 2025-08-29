using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SignalR.Models
{
    public class SignalrSetting
    {
        /// <summary>
        /// url
        /// </summary>
        public   string url { get; set; }

        /// <summary>
        /// 配置频道
        /// </summary>
        public   string configurationChannel { get; set; }

        /// <summary>
        ///主机名
        /// </summary>

        public   string hostname { get; set; }


        /// <summary>
        ///端口
        /// </summary>

        public   string port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public   string password { get; set; }
    }
}
