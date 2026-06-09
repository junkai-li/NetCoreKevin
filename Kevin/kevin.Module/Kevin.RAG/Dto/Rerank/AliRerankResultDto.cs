using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kevin.RAG.Dto.Rerank
{
    /// <summary>
    /// 阿里重排结果
    /// </summary>
    public class AliRerankResultDto
    {
        [JsonPropertyName("error")]
        public ErrorDetail Error { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("results")]
        public List<ResultItem> Results { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    public class ResultItem
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("relevance_score")]
        public double RelevanceScore { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }


    public class ErrorDetail
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("param")]
        public string Param { get; set; }  // 可能为 null

        [JsonPropertyName("code")]
        public string Code { get; set; }
    }

}
