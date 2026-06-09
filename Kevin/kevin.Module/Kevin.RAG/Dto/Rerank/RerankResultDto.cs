using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Dto.Rerank
{
    public class RerankResultDto
    {
        public List<RerankResultItemDto> results { get; set; } = new List<RerankResultItemDto>();
    }

    public class RerankResultItemDto
    {
        /// <summary>
        /// 文档索引，表示该文档在原始输入列表中的位置
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 相关性得分，表示该文档与查询的相关程度
        /// </summary>
        public double relevance_score { get; set; }
    }
}
