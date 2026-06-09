using Kevin.RAG.Dto;
using Microsoft.Extensions.AI;

namespace Kevin.RAG.Interfaces
{
    public interface IRAGService
    {

        /// <summary>
        /// 获取向量搜索服务的并拼接系统提示词返回
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="question">查询问题</param>
        /// <param name="IsRerankModel">是否使用重排序模型</param>   
        /// <param name="topK">返回的最相关文档数量</param>
        /// <param name="Score">相关性得分阈值</param>
        /// <returns></returns>
        Task<(bool, string, List<DocumentChunkDto>)> GetRAGSystemPrompt(string collectionName, Embedding<float> question, string questionText = "", bool isRerankModel = false, int topK = 3, double? Score = null);

        /// <summary>
        /// 获取向量搜索服务和Ali重排序服务并拼接系统提示词返回
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="question">查询问题</param> 
        /// <param name="topK">返回的最相关文档数量</param>
        /// <param name="Score">相关性得分阈值</param>
        /// <param name="RerankUrl">重排序服务的URL</param>
        /// <param name="RerankKey">重排序服务的API Key</param>
        /// <param name="RerankModel">重排序服务的模型名称</param>
        /// <returns></returns>
        Task<(bool, string, List<DocumentChunkDto>)> GetRAGAliReankSystemPrompt(string collectionName, Embedding<float> question, string questionText = "", int topK = 3, double? Score = null, string RerankUrl = "", string RerankKey = "", string RerankModel = "");
    }
}
