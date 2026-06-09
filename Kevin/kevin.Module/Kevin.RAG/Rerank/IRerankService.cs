using Kevin.RAG.Dto.Rerank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Rerank
{
    public interface IRerankService : IDisposable
    { 
        /// <summary>
        /// 重排接口，输入query和一组候选文档，输出重排结果输出重排后的文档列表
        /// </summary>
        /// <param name="query">提问词</param>
        /// <param name="documents">文档列表</param>
        /// <param name="modelUrl">模型URL</param>
        /// <param name="modelName">模型名称</param>
        /// <param name="modelKey">模型密钥</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>重排结果</returns>
        public Task<RerankResultDto> RerankAsync(string query, List<string> documents, int topK = 0, CancellationToken? cancellationToken = default);
    }
}
