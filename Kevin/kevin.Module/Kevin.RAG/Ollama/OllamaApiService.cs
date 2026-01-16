using Elastic.Transport;
using Kevin.RAG.Ollama.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace Kevin.RAG.Ollama
{
    public class OllamaApiService : IOllamaApiService
    {
        private readonly string Url;
        private readonly string DefaultModel;
        private readonly string Key;
        private readonly OllamaApiClient ollamaApiClient;

        public OllamaApiService(IOptionsMonitor<OllamaApiSetting> config)
        {
            try
            {
                Url = config.CurrentValue.Url;
                DefaultModel = config.CurrentValue.DefaultModel;
                Key= config.CurrentValue.ApiKey;
                if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(DefaultModel))
                {
                    ollamaApiClient = new OllamaApiClient(new Uri(Url), DefaultModel);
                   
                }
                if (!string.IsNullOrEmpty(Key))
                {
                    // 注意：原生Ollama通常不需要ApiKey，这里展示如何通过HttpClient添加认证头
                    ollamaApiClient?.DefaultRequestHeaders.Add("Authorization", "Bearer "+ Key);
                }
              
            }
            catch (Exception)
            {

                Console.WriteLine("Kevin.RAG请检查OllamaApi配置是否正确");
            }

        }
        public async Task<Embedding<float>> GetEmbedding(string text)
        {
            if (ollamaApiClient == default)
            {
                throw new ArgumentException($"请检查OllamaApi配置是否正确");
            } 
            return await Microsoft.Extensions.AI.EmbeddingGeneratorExtensions.GenerateAsync<string, Embedding<float>>(ollamaApiClient, text, options: null, cancellationToken: default).ConfigureAwait(false);
        }
    }
}
