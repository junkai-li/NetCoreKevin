using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// token消费信息
    /// </summary>
    public class TokenConsumptionInfo
    {
        /// <summary>
        /// 输入消耗的token数
        /// </summary>
        public long? InputTokenCount { get; set; }

        /// <summary>
        /// 输出消耗的token数
        /// </summary>
        public long? OutputTokenCount { get; set; }

        /// <summary>
        /// 总消耗的token数
        /// </summary>
        public long? TotalTokenCount { get; set; }

        /// <summary>
        /// 缓存中读取的输入标记的数量 缓存的输入标记应计入<see cref="InputTokenCount"/>中。
        /// </summary>
        public long? CachedInputTokenCount { get; set; }

        /// <summary>
        /// “推理”/“思考”标记的数量
        /// by the model.
        /// </summary>
        /// <remarks>
        /// 推理标记应计入<see cref="OutputTokenCount"/>中。
        /// </remarks>
        public long? ReasoningTokenCount { get; set; }

    }
}
