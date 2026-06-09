using Kevin.Common.Extension;
using Kevin.HttpApiClients.Helper;
using Kevin.RAG.Dto;
using Kevin.RAG.Dto.Rerank;
using Kevin.RAG.Ollama.Models;
using Kevin.RAG.Rerank.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Rerank
{
    /// <summary>
    /// 重排序服务
    /// </summary>
    public class AliRerankService : IRerankService
    {
        private readonly HttpClientDisHelper httpClientDisHelper;
        private readonly string Url;
        private readonly string DefaultModel;
        private readonly string Key;

        public AliRerankService(string _Url, string _DefaultModel, string _Key)
        {
            httpClientDisHelper = new HttpClientDisHelper();
            Url = _Url;
            DefaultModel = _DefaultModel;
            Key = _Key;
            if (!string.IsNullOrEmpty(Key))
            {
                httpClientDisHelper.AddHeader("Authorization", "Bearer " + Key);
            }
        }

        public AliRerankService(IOptionsMonitor<RerankApiSetting> config)
        {
            httpClientDisHelper = new HttpClientDisHelper();
            Url = config.CurrentValue.Url;
            DefaultModel = config.CurrentValue.DefaultModel;
            Key = config.CurrentValue.ApiKey;
            if (!string.IsNullOrEmpty(Key))
            {
                httpClientDisHelper.AddHeader("Authorization", "Bearer " + Key);
            }
        }
        public async Task<RerankResultDto> RerankAsync(string query, List<string> documents, int topK = 0, CancellationToken? cancellationToken = null)
        {
            if (httpClientDisHelper == default)
            {
                throw new ArgumentException($"请检查AliRerankService配置是否正确");
            }
            var reslut = new RerankResultDto();
            if (documents.Count == 0)
            {
                return reslut;
            }
            if (topK <= 0)
            {
                topK = documents.Count;
            }
            var data = (await httpClientDisHelper.PostAsync<AliRerankResultDto>(Url,
               new
               {
                   model = DefaultModel,
                   documents = documents,
                   query = query,
                   top_n = topK,
                   instruct = "Given a web search query, retrieve relevant passages that answer the query."
               }.ToJson()));
            if (data != null && data.Error != default && !string.IsNullOrEmpty(data.Error.Message))
            {
                throw new ArgumentException($"{data.Error.Message}");
            }
            if (data != null && data.Results != null)
            {
                reslut.results = data.Results.Select(x => new RerankResultItemDto
                {
                    index = x.Index,
                    relevance_score = x.RelevanceScore,
                }).ToList();
            }
            return reslut;
        }

        public void Dispose()
        {
            if (httpClientDisHelper != default)
            {
                httpClientDisHelper.Dispose();
            }
        }
    }
}
