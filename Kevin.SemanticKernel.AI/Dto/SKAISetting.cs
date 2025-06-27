using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.SemanticKernel.Dto
{
    public class SKAISetting
    {
        /// <summary>
        /// 模型名称
        /// </summary>
        public string modelId { get; set; } = "glm-4-flash";

        /// <summary>
        /// 默认使用智谱AI的模型，如果需要使用自己的模型，请修改endpoint和apiKey
        /// </summary>
        public string endpoint { get; set; } = "https://open.bigmodel.cn/api/paas/v4/";

            /// <summary>
            ///根据自己的APIKEY进行修改
            /// </summary>
        public string apiKey { get; set; } = "******";
    }
}
