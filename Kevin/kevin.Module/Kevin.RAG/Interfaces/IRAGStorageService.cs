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
            Embedding<float> query, ulong limit = 20, double? Score = null);

        /// <summary>
        /// 按来源文件删除向量点（知识库移除单个文件时使用，其他文件的块不受影响）
        /// </summary>
        /// <param name="collectionName">集合名</param>
        /// <param name="sourceFile">来源文件名（写入时落在 payload 的 SourceFile 上）</param>
        /// <returns></returns>
        Task<bool> DeleteData(string collectionName, string sourceFile);

        /// <summary>
        /// 删除整个集合（删除知识库时使用）
        /// </summary>
        /// <param name="collectionName">集合名</param>
        /// <returns></returns>
        Task<bool> DeleteCollection(string collectionName);

        /// <summary>
        /// 按来源文件分页拉取文档碎片（知识库文件碎片预览使用，实时 Scroll 查询，不依赖向量）
        /// </summary>
        /// <param name="collectionName">集合名</param>
        /// <param name="sourceFile">来源文件名（写入时落在 payload 的 SourceFile 上）</param>
        /// <param name="limit">每页条数</param>
        /// <param name="offset">偏移量（按 ChunkIndex 升序跳过的块数）</param>
        /// <returns>碎片总数与当前页碎片列表（按 ChunkIndex 升序）</returns>
        Task<(ulong Total, List<DocumentChunkDto> Chunks)> GetChunksBySourceFile(
            string collectionName, string sourceFile, ulong limit, ulong offset);

    }
}
