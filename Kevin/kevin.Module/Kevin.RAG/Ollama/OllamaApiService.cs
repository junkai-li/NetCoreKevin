using Kevin.Common.Extension;
using Kevin.HttpApiClients.Helper;
using Kevin.RAG.Dto;
using Kevin.RAG.Ollama.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using System.Text.Json;

namespace Kevin.RAG.Ollama
{
    public class OllamaApiService : IOllamaApiService
    {
        private readonly string? Url;
        private readonly string? DefaultModel;
        private readonly string? Key;
        private readonly OllamaApiClient? ollamaApiClient;
        private readonly HttpClientDisHelper? httpClientDisHelper;
        public OllamaApiService(string _Url, string _DefaultModel, string _Key)
        {
            try
            {
                Url = _Url;
                DefaultModel = _DefaultModel;
                Key = _Key;
                if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(DefaultModel))
                {
                    ollamaApiClient = new OllamaApiClient(new Uri(Url), DefaultModel);

                }
                if (!string.IsNullOrEmpty(Key))
                {
                    // 注意：原生Ollama通常不需要ApiKey，这里展示如何通过HttpClient添加认证头
                    ollamaApiClient?.DefaultRequestHeaders.Add("Authorization", "Bearer " + Key);
                    httpClientDisHelper = new HttpClientDisHelper();
                    httpClientDisHelper.AddHeader("Authorization", "Bearer " + Key);
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Kevin.RAG请检查OllamaApi配置是否正确");
            }

        }
        public OllamaApiService(IOptionsMonitor<OllamaApiSetting> config)
        {
            try
            {
                Url = config.CurrentValue.Url;
                DefaultModel = config.CurrentValue.DefaultModel;
                Key = config.CurrentValue.ApiKey;
                if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(DefaultModel))
                {
                    ollamaApiClient = new OllamaApiClient(new Uri(Url), DefaultModel);

                }
                if (!string.IsNullOrEmpty(Key))
                {
                    // 注意：原生Ollama通常不需要ApiKey，这里展示如何通过HttpClient添加认证头
                    ollamaApiClient?.DefaultRequestHeaders.Add("Authorization", "Bearer " + Key);
                    httpClientDisHelper = new HttpClientDisHelper();
                    httpClientDisHelper.AddHeader("Authorization", "Bearer " + Key);
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
            if (!string.IsNullOrEmpty(Key))
            {
                if (httpClientDisHelper != default)
                {
                    return await GetEmbeddingViaHttpApi(text);
                }
            }
            return await Microsoft.Extensions.AI.EmbeddingGeneratorExtensions.GenerateAsync<string, Embedding<float>>(ollamaApiClient, text, options: null, cancellationToken: default).ConfigureAwait(false);

        }

        /// <summary>
        /// 经 OpenAI 兼容接口获取嵌入向量（带 Key 的第三方兼容网关走这条通道）。
        /// <para>
        /// 修复记录：旧实现直接把 EndPoint 当 POST 地址（缺少 /embeddings 路径，请求打到网关根路径），
        /// 且 HttpClientDisHelper.PostAsync 不校验状态码，错误响应反序列化成空 data 数组后
        /// data.data.First() 抛出隐晦的 "Sequence contains no elements"——知识库所有文件类型
        /// （xlsx/Text/PDF/Word…）共用此向量化环节，与文件类型无关。
        /// 现改为：EndPoint 自动补 /embeddings 路径；先取原始响应再反序列化，
        /// 非 JSON / data 为空 / 向量维度为 0 都抛出带响应片段的可读异常，便于直接定位配置问题。
        /// </para>
        /// </summary>
        private async Task<Embedding<float>> GetEmbeddingViaHttpApi(string text)
        {
            var requestUrl = BuildEmbeddingsUrl(Url);
            // 用原始 POST（不反序列化）：PostAsync<string> 会把 JSON 对象响应当 string 反序列化而抛
            // "The JSON value could not be converted to System.String"
            var (statusCode, raw) = await httpClientDisHelper!.PostAsyncRaw(requestUrl, new { model = DefaultModel, input = text }.ToJson());

            if (statusCode < 200 || statusCode >= 300)
                throw new Exception($"嵌入向量接口返回 HTTP {statusCode}，请检查模型 EndPoint/密钥是否正确: {requestUrl}，响应片段: {TruncateResponseForError(raw)}");

            var trimmed = (raw ?? string.Empty).Trim();
            if (trimmed.Length == 0 || !trimmed.StartsWith("{"))
                throw new Exception($"嵌入向量接口返回非 JSON 响应，请检查模型 EndPoint 是否为 OpenAI 兼容地址（如 https://host/v1）: {requestUrl}，响应片段: {TruncateResponseForError(trimmed)}");

            EmbeddingDto? data;
            try
            {
                data = JsonSerializer.Deserialize<EmbeddingDto>(trimmed, EmbeddingJsonOptions);
            }
            catch (JsonException)
            {
                throw new Exception($"嵌入向量接口返回无法按 OpenAI 兼容格式解析: {requestUrl}，响应片段: {TruncateResponseForError(trimmed)}");
            }

            var vector = data?.data?.FirstOrDefault()?.embedding;
            if (vector == null || vector.Count == 0)
                throw new Exception($"嵌入向量接口未返回向量数据，请检查模型名称/密钥/EndPoint 是否正确（常见：模型不具备 Embedding 能力或密钥无权限）: {requestUrl}，响应片段: {TruncateResponseForError(trimmed)}");

            return new Embedding<float>(vector.ToArray().AsMemory());
        }

        /// <summary>EndPoint 规范化为 /embeddings 完整地址：已以 /embeddings 结尾（兼容手工配置了完整地址）不再追加</summary>
        private static string BuildEmbeddingsUrl(string? url)
        {
            var trimmed = (url ?? string.Empty).TrimEnd('/');
            return trimmed.EndsWith("/embeddings", StringComparison.OrdinalIgnoreCase)
                ? trimmed
                : trimmed + "/embeddings";
        }

        /// <summary>错误消息里携带的响应片段：去换行截 200 字符，足够识别鉴权/模型名错误又不撑爆消息</summary>
        private static string TruncateResponseForError(string response)
        {
            var compact = response.Replace("\r", "").Replace("\n", " ");
            return compact.Length <= 200 ? compact : compact.Substring(0, 200) + "...";
        }

        /// <summary>嵌入响应反序列化配置：兼容网关字段大小写不一致</summary>
        private static readonly JsonSerializerOptions EmbeddingJsonOptions = new() { PropertyNameCaseInsensitive = true };

        public void Dispose()
        {
            ollamaApiClient?.Dispose();
            httpClientDisHelper?.Dispose();
        }
    }
}
