using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Kevin.RAG.Ollama.Models;
using Kevin.RAG.Qdrant;
using Kevin.RAG.Qdrant.Models;
using Kevin.RAG.Rerank;
using Kevin.RAG.Rerank.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static void AddRAGQdrantAndOllama(this IServiceCollection services, Action<OllamaApiSetting> actionOllamaApi, Action<QdrantClientSetting> actionQdrantClient, Action<RerankApiSetting>? actionRerank = default
            )
        {
            services.Configure(actionOllamaApi);
            services.Configure(actionQdrantClient);
            if (actionRerank != default)
            {
                services.Configure(actionRerank);
            }
            services.TryAddScoped<IOllamaApiService, OllamaApiService>();
            services.TryAddScoped<IRAGStorageService, QdrantClientService>();
            services.TryAddScoped<IRAGService, RAGService>();
            services.TryAddScoped<IRerankService, AliRerankService>();
        }
    }
}
