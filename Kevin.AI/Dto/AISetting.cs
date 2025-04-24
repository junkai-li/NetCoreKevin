using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.Dto
{
    public class AISetting
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string AIUrl { get; set; }  


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string AIKeySecret { get; set; }

        /// <summary>
        /// 默认模型
        /// </summary>
        public string AIDefaultModel { get; set; }
    }
}
