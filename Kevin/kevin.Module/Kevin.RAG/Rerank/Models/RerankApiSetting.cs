using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Rerank.Models
{
    public class RerankApiSetting
    {   
        /// <summary>
        /// 模型Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 模型密钥
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string DefaultModel { get; set; }
    }
}
