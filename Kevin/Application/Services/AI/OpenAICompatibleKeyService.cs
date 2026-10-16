using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.Common;
using Kevin.log4Net;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// OpenAI 兼容接口的用户 Key 服务实现：
    /// <list type="bullet">
    ///   <item><description>每个用户（UserId + TenantId）至多一条 Key；唯一约束由 UserId 索引保证</description></item>
    ///   <item><description>Key 落库为 AES 加密后的密文（<see cref="Kevin.Common.AESEncryptor.EncryptString"/>）；外部命中由网关层解密匹配</description></item>
    ///   <item><description>Key 格式 <c>sk-kv-{Base62(48字节随机)}</c>，用 <see cref="RandomNumberGenerator"/> 生成加密安全随机数</description></item>
    /// </list>
    /// </summary>
    public class OpenAICompatibleKeyService : BaseService, IOpenAICompatibleKeyService
    {
        public ITAIOpenAICompatibleKeyRp TAIOpenAICompatibleKeyRp { get; set; }

        public IAIAppsService AIAppsService { get; set; }

        public OpenAICompatibleKeyService(
            IHttpContextAccessor _httpContextAccessor,
            ITAIOpenAICompatibleKeyRp _TAIOpenAICompatibleKeyRp,
            IAIAppsService _AIAppsService) : base(_httpContextAccessor)
        {
            this.TAIOpenAICompatibleKeyRp = _TAIOpenAICompatibleKeyRp;
            this.AIAppsService = _AIAppsService;
        }

        private const string KeyPrefixText = "sk-kv";

        public async Task<OpenAICompatibleKeyInfoDto> GetMyKey()
        {
            var entity = await GetMyEntityAsync();
            if (entity == null)
            {
                return new OpenAICompatibleKeyInfoDto { IsExists = false };
            }
            return new OpenAICompatibleKeyInfoDto
            {
                IsExists = true,
                Id = entity.Id,
                UserId = entity.UserId,
                MaskKey = BuildMask(entity.KeyPrefix, entity.KeySuffix),
                Name = entity.Name,
                IsEnabled = entity.IsEnabled,
                CreateTime = entity.CreateTime,
                LastUsedTime = entity.LastUsedTime,
            };
        }

        public async Task<string> GenerateMyKey(string? name)
        {
            if (CurrentUser == null || CurrentUser.UserId <= 0)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            // 已存在：作废旧 Key（与代码库软删除惯例一致，保留审计痕迹）。GetMyEntityAsync 过滤 IsDelete=false，新 Key 即生效
            var existing = await GetMyEntityAsync();
            if (existing != null)
            {
                existing.IsDelete = true;
                existing.DeleteTime = DateTime.Now;
                existing.IsEnabled = false;
            }

            var plainKey = GeneratePlainKey();
            var suffix = plainKey.Length >= 4 ? plainKey[^4..] : plainKey;

            var entity = new TAIOpenAICompatibleKey
            {
                Id = SnowflakeIdService.GetNextId(),
                UserId = CurrentUser.UserId,
                EncryptedKey = AESEncryptor.EncryptString(plainKey),
                KeyPrefix = KeyPrefixText,
                KeySuffix = suffix,
                Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
                IsEnabled = true,
                CreateTime = DateTime.Now,
                TenantId = CurrentUser.TenantId,
            };
            TAIOpenAICompatibleKeyRp.Add(entity);
            await TAIOpenAICompatibleKeyRp.SaveChangesAsync();

            return plainKey;
        }

        public async Task<string> RevealMyKey()
        {
            var entity = await GetMyEntityAsync()
                ?? throw new UserFriendlyException("尚未生成 OpenAI 兼容接口 Key");
            if (string.IsNullOrEmpty(entity.EncryptedKey))
            {
                throw new UserFriendlyException("Key 数据异常");
            }
            try
            {
                return AESEncryptor.DecryptString(entity.EncryptedKey);
            }
            catch (Exception ex)
            {
                Kevin.log4Net.LogHelper.logger.Error("OpenAI兼容接口 Key 解密失败：" + ex.Message, ex);
                throw new UserFriendlyException("Key 解密失败，请重新生成");
            }
        }

        public async Task<bool> UpdateMyKeyName(string? name)
        {
            var entity = await GetMyEntityAsync()
                ?? throw new UserFriendlyException("尚未生成 OpenAI 兼容接口 Key");
            entity.Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            entity.UpdateTime = DateTime.Now;
            await TAIOpenAICompatibleKeyRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetMyKeyEnabled(bool enabled)
        {
            var entity = await GetMyEntityAsync()
                ?? throw new UserFriendlyException("尚未生成 OpenAI 兼容接口 Key");
            entity.IsEnabled = enabled;
            entity.UpdateTime = DateTime.Now;
            await TAIOpenAICompatibleKeyRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMyKey()
        {
            var entity = await GetMyEntityAsync();
            if (entity == null) return true;
            // 软删除：保持与代码库惯例一致（DeleteTime + IsDelete 均为 CD 基类字段）
            entity.IsDelete = true;
            entity.DeleteTime = DateTime.Now;
            entity.IsEnabled = false;
            await TAIOpenAICompatibleKeyRp.SaveChangesAsync();
            return true;
        }

        public async Task<OpenAICompatibleIntegrationDocDto> GetMyIntegrationDoc(string baseUrl)
        {
            var myApps = await AIAppsService.GetMyALLList();
            var doc = new OpenAICompatibleIntegrationDocDto
            {
                BaseUrl = (baseUrl ?? "").TrimEnd('/'),
                Models = myApps
                    .Where(t => t.Id > 0 && !string.IsNullOrWhiteSpace(t.Name))
                    .Select(t => new OpenAICompatibleIntegrationModelItem
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Describe = t.Describe,
                    })
                    .ToList(),
            };

            // 占位 key 与模型，文档展示时再替换成真实值（前端在复制按钮回调中替换）
            doc.Samples = new OpenAICompatibleIntegrationSamples
            {
                Curl = BuildCurlSample(doc.BaseUrl, "<your-api-key>", "<model id>"),
                Python = BuildPythonSample(doc.BaseUrl, "<your-api-key>", "<model id>"),
                NodeJs = BuildNodeJsSample(doc.BaseUrl, "<your-api-key>", "<model id>"),
            };
            return doc;
        }

        // ===== 内部辅助 =====

        private Task<TAIOpenAICompatibleKey?> GetMyEntityAsync()
        {
            if (CurrentUser == null || CurrentUser.UserId <= 0)
            {
                return Task.FromResult<TAIOpenAICompatibleKey?>(null);
            }
            return TAIOpenAICompatibleKeyRp.Query(isDataPer: false)
                .Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.UserId == CurrentUser.UserId)
                .FirstOrDefaultAsync()!;
        }

        private static string BuildMask(string prefix, string suffix)
        {
            prefix = string.IsNullOrEmpty(prefix) ? KeyPrefixText : prefix;
            suffix = string.IsNullOrEmpty(suffix) ? "****" : suffix;
            return $"{prefix}-••••••••••••{suffix}";
        }

        /// <summary>
        /// 生成明文 Key：sk-kv + 48 字节加密安全随机数 → Base62 编码。
        /// Base62 输出长度 ≈ 86 字符，加上前缀约 92 字符，安全稳定。
        /// </summary>
        private static string GeneratePlainKey()
        {
            Span<byte> bytes = stackalloc byte[48];
            RandomNumberGenerator.Fill(bytes);
            return KeyPrefixText + ToBase62(bytes);
        }

        private static string ToBase62(ReadOnlySpan<byte> bytes)
        {
            const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var sb = new System.Text.StringBuilder(bytes.Length * 2);
            // 把字节流整体当作大整数，再反复对 62 取模得到每位字符
            var n = System.Numerics.BigInteger.Zero;
            foreach (var b in bytes) n = (n << 8) | b;
            if (n == 0) return "0";
            while (n > 0)
            {
                var r = (int)(n % 62);
                sb.Insert(0, alphabet[r]);
                n /= 62;
            }
            return sb.ToString();
        }

        private static string BuildCurlSample(string baseUrl, string key, string model)
        {
            // shell 行续：verbatim 字符串里写单反斜杠（续行符 \ + 换行）；前两行用 "" 转义字面双引号
            return
$@"curl ""{baseUrl}/chat/completions"" \
  -H ""Content-Type: application/json"" \
  -H ""Authorization: Bearer {key}"" \
  -d '{{
      ""model"": ""{model}"",
      ""messages"": [
        {{ ""role"": ""system"", ""content"": ""You are a helpful assistant."" }},
        {{ ""role"": ""user"", ""content"": ""你好"" }}
      ]
    }}'";
        }

        private static string BuildPythonSample(string baseUrl, string key, string model)
        {
            return
$@"# pip install openai
from openai import OpenAI

client = OpenAI(
    api_key=""{key}"",
    base_url=""{baseUrl}"",
)

resp = client.chat.completions.create(
    model=""{model}"",
    messages=[
            {{ ""role"": ""system"", ""content"": ""You are a helpful assistant."" }},
            {{ ""role"": ""user"", ""content"": ""你好"" }},
        ],
)
print(resp.choices[0].message.content)";
        }

        private static string BuildNodeJsSample(string baseUrl, string key, string model)
        {
            return
$@"// npm install openai
import OpenAI from ""openai"";

const client = new OpenAI({{
  apiKey: ""{key}"",
  baseURL: ""{baseUrl}"",
}});

const completion = await client.chat.completions.create({{
  model: ""{model}"",
  messages: [
    {{ role: ""system"", content: ""You are a helpful assistant."" }},
    {{ role: ""user"", content: ""你好"" }},
  ],
}});
console.log(completion.choices[0].message.content);";
        }
    }
}