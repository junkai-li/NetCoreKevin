using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SMS.AliCloud.Models
{
    public class SMSSetting
    {


        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccessKeyId { get; set; } = "";


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string AccessKeySecret { get; set; } = "";


    }
}
