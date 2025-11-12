using Microsoft.Extensions.AI;
using OllamaSharp;

namespace Kevin.AI.SemanticKernel.RAG
{
    public class LLMUtils
    {
        static OllamaApiClient api = new OllamaApiClient(new Uri("http://****/v1"), "modelscope.cn/Qwen/Qwen3-Embedding-0.6B-GGUF:latest");

        /// <summary>
        /// 获得文本的编码
        /// bge-m3 的长度是1024 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async static Task<Embedding<float>> GetEmbedding(string text)
        {
            // 明确指定扩展方法，避免二义性
            return await Microsoft.Extensions.AI.EmbeddingGeneratorExtensions.GenerateAsync<string, Embedding<float>>(api, text, options: null, cancellationToken: default).ConfigureAwait(false);
        }


    }
}
