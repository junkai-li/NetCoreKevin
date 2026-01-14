using Kevin.RAG.Ollama;
using Kevin.RAG.Ollama.Models;
using Kevin.RAG.Qdrant;
using Kevin.RAG.Qdrant.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Kevin.RAG
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加RAG搜索AI增强 服务，使用Ollama作为语言模型，Qdrant作为向量数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="actionOllamaApi"></param>
        /// <param name="actionQdrantClient"></param>
        public static void AddRAGQdrantAndOllama(this IServiceCollection services, Action<OllamaApiSetting> actionOllamaApi, Action<QdrantClientSetting> actionQdrantClient)
        {
            services.Configure(actionOllamaApi);
            services.Configure(actionQdrantClient);
            services.AddTransient<IOllamaApiService, OllamaApiService>();
            services.AddTransient<IQdrantClientService, QdrantClientService>();
            services.AddTransient<IRAGService, RAGService>();
        }
    }
}
