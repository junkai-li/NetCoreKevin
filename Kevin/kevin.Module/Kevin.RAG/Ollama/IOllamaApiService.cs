using Microsoft.Extensions.AI;

namespace Kevin.RAG.Ollama
{
    public interface IOllamaApiService:IDisposable
    {
        /// <summary>
        /// 返回矢量数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<Embedding<float>> GetEmbedding(string text);
    }
}
