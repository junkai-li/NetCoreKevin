using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Asr.AliCloud.Models
{
    public class AliAsrSetting
    {
        // <summary>
        /// 账户ID
        /// </summary>
        public string AccessKeyId { get; set; } = "";


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string AccessKeySecret { get; set; } = "";
        /// <summary>
        /// 项目ID
        /// </summary>

        public string AsrAppKey { get; set; } = "";
    }
}
