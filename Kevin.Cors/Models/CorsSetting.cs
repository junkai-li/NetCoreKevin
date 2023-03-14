using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Cors.Models
{
    public class CorsSetting
    {
        /// <summary>
        /// 是否所有ip
        /// </summary>
        public bool EnableAllIPs { get; set; }

        /// <summary>
        /// 策略名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IPs { get; set; }
    }
}
