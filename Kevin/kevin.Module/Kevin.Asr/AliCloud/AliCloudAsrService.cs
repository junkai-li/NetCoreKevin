using Kevin.Asr.AliCloud.Models;
using Kevin.Asr.Dto;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Kevin.Asr.AliCloud
{
    public class AliCloudAsrService : IAsrService
    {
        private readonly string _accessKeyId;


        private readonly string _accessKeySecret; 

        private readonly string _asrAppKey;
        private readonly HttpClient _httpClient;
        public AliCloudAsrService(IOptionsMonitor<AliAsrSetting> config, IHttpClientFactory httpClientFactory)
        {
            _accessKeyId = config.CurrentValue.AccessKeyId;
            _accessKeySecret = config.CurrentValue.AccessKeySecret;
            _asrAppKey = config.CurrentValue.AsrAppKey;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 通过阿里云 OpenAPI 获取 NLS Token（有效期 24 小时）
        /// 参考文档: https://help.aliyun.com/zh/isi/getting-started/use-http-or-https-to-obtain-an-access-token
        /// </summary>
        public async Task<AsrTokenResultDto> GenerateTokenAsync()
        {
            // 构造请求参数
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            var signatureNonce = Guid.NewGuid().ToString("N").Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            var parameters = new Dictionary<string, string>
            {
                ["AccessKeyId"] = _accessKeyId,
                ["Action"] = "CreateToken",
                ["Format"] = "JSON",
                ["RegionId"] = "cn-shanghai",
                ["SignatureMethod"] = "HMAC-SHA1",
                ["SignatureNonce"] = signatureNonce,
                ["SignatureVersion"] = "1.0",
                ["Timestamp"] = timestamp,
                ["Version"] = "2019-02-28"
            };

            // 排序并编码参数
            var sortedParameters = parameters.OrderBy(kv => kv.Key);
            var canonicalizedQueryString = string.Join("&", sortedParameters.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            // 构造待签名字符串
            var stringToSign = $"GET&%2F&{Uri.EscapeDataString(canonicalizedQueryString)}";

            // 计算签名
            var signature = ComputeSignature(stringToSign, _accessKeySecret);

            // 添加签名到参数
            parameters.Add("Signature", signature);

            // 构造最终 URL
            var url = $"http://nls-meta.cn-shanghai.aliyuncs.com/?{string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                if (result?.Token?.Id != null && result?.Token?.ExpireTime != null)
                {
                    return new AsrTokenResultDto
                    {
                        AppKey = _asrAppKey,
                        Token = result.Token.Id,
                        ExpireTime = result.Token.ExpireTime
                    };
                }

                throw new InvalidOperationException($"Invalid token response: {json}");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to obtain Aliyun ASR token: {ex.Message}", ex);
            }
        }

        private string ComputeSignature(string stringToSign, string accessKeySecret)
        {
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(accessKeySecret + "&"));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            return Convert.ToBase64String(hash);
        }
    }
}
