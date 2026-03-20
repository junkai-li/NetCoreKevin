using Kevin.RAG.Dto;
using Microsoft.Extensions.AI;

namespace Kevin.RAG.Interfaces
{
    public interface IRAGStorageService : IDisposable
    {

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddData(string collectionName, List<DocumentChunkDto> data, ulong embeddingValueSize = 2048);

        /// <summary>
        ///  搜索向量数据 向量搜索：根据查询向量找到最相关的文档
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <param name="limit">返回多少数据</param>
        /// <param name="Score">相似度</param> 
        /// <returns></returns>
        Task<List<DocumentChunkDto>> Search(string collectionName,
            Embedding<float> query, ulong limit = 10, double? Score = null);

    }
}
