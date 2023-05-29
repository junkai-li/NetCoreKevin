using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SMS.TencentCloud.Models
{
    public class SMSSetting
    {


        /// <summary>
        /// SDK AppId (非账号APPId)
        /// </summary>
        public string AppId { get; set; }


        /// <summary>
        /// 账号密钥ID
        /// </summary>
        public string SecretId { get; set; }


        /// <summary>
        /// 账号密钥Key
        /// </summary>
        public string SecretKey { get; set; }
    }
}
