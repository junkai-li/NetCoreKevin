using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using Kevin.RAG.Ollama;
using Kevin.RAG.Qdrant.Models;
using Kevin.RAG.Rerank;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 智能体记忆向量服务 —— 基于 Qdrant 向量数据库的语义检索层。
    /// <para>
    /// 由 <see cref="AIAgentMemoryService"/> 调用，作为记忆的「优先通道」：
    /// 写入时同步生成向量并存入 Qdrant，搜索时优先语义检索，失败时由调用方降级到数据库关键词搜索。
    /// </para>
    /// <para>
    /// 依赖链：OllamaApiService（Embedding 生成）→ QdrantClient（向量存取）→ AliRerankService（可选重排）
    /// </para>
    /// </summary>
    public class AIQdrantAgentMemoryService : IAIQdrantAgentMemoryService
    {
        #region 字段与构造

        private readonly QdrantClient? _qdrantClient;
        private readonly IOllamaApiService? _ollamaApiService;
        private readonly IRerankService? _rerankService;
        private readonly int _embeddingSize;

        /// <summary>
        /// 记忆集合名前缀，完整名称为 {前缀}_{tenantId}_{aiAppsId}_{userId}（租户+智能体+用户三维隔离）
        /// </summary>
        private const string CollectionPrefix = "ai_memory";

        /// <summary>
        /// 搜索时从 Qdrant 拉取的候选数（大于最终返回数，留出防御性校验与 Rerank 重排余量）
        /// </summary>
        private const int SearchFetchLimit = 50;

        /// <summary>
        /// 最终返回给调用方的最大记忆条数
        /// </summary>
        private const int MaxReturnCount = 10;

        public AIQdrantAgentMemoryService(IServiceProvider serviceProvider)
        {
            // 从服务容器安全解析依赖：未注册时为 null，服务自动降级而非启动报错
            _ollamaApiService = serviceProvider.GetService<IOllamaApiService>();
            _rerankService = serviceProvider.GetService<IRerankService>();

            var qdrantConfig = serviceProvider.GetService<IOptionsMonitor<QdrantClientSetting>>();
            if (qdrantConfig == null) return;

            _embeddingSize = qdrantConfig.CurrentValue.MemoryEmbeddingSize;

            var url = qdrantConfig.CurrentValue.Url;
            if (string.IsNullOrEmpty(url)) return;

            try
            {
                var apiKey = qdrantConfig.CurrentValue.ApiKey;
                var thumbprint = qdrantConfig.CurrentValue.CertificateThumbprint;

                if (!string.IsNullOrEmpty(apiKey))
                {
                    var channel = QdrantChannel.ForAddress(url, new ClientConfiguration
                    {
                        ApiKey = apiKey,
                        CertificateThumbprint = thumbprint
                    });
                    _qdrantClient = new QdrantClient(new QdrantGrpcClient(channel));
                }
                else
                {
                    _qdrantClient = new QdrantClient(url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIQdrantMemory] Qdrant 客户端初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 服务是否可用（Qdrant 客户端已初始化 且 Embedding 服务已注册）
        /// </summary>
        public bool IsAvailable => _qdrantClient != null && _ollamaApiService != null;

        /// <summary>
        /// 获取非空 Qdrant 客户端（调用前必须先检查 IsAvailable）
        /// </summary>
        private QdrantClient Client => _qdrantClient!;

        #endregion

        #region 向量写入

        /// <summary>
        /// 将记忆向量写入 Qdrant（新增或覆盖，使用 Upsert）。
        /// 调用方应先完成数据库持久化，再调用本方法同步向量。
        /// </summary>
        public async Task UpsertMemoryVectorAsync(TAIAgentMemory memory)
        {
            if (!IsAvailable) return;

            var embedding = await _ollamaApiService!.GetEmbedding(BuildEmbeddingText(memory.Content, memory.Keywords));
            var collectionName = GetCollectionName(memory.TenantId, memory.AIAppsId, memory.UserId);

            await EnsureCollectionExistsAsync(collectionName);

            var point = new PointStruct
            {
                Id = (ulong)memory.Id,
                Vectors = embedding.Vector.ToArray(),
                Payload =
                {
                    ["userId"] = memory.UserId.ToString(),
                    ["aiAppsId"] = memory.AIAppsId.ToString(),
                    ["tenantId"] = memory.TenantId.ToString(),
                    ["memoryType"] = memory.MemoryType ?? "other",
                    ["keywords"] = memory.Keywords ?? "",
                    ["content"] = memory.Content ?? "",
                    ["importance"] = memory.Importance.ToString(),
                    ["createTime"] = memory.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            await Client.UpsertAsync(collectionName, new List<PointStruct> { point });
        }

        #endregion

        #region 向量搜索

        /// <summary>
        /// 语义搜索记忆（Qdrant 向量检索 + 可选 Rerank 重排）。
        /// <para>
        /// 流程：生成查询向量 → Qdrant 相似度检索 → 防御性校验(userId/tenantId/aiAppsId) → Rerank 重排 → 取 TopN
        /// </para>
        /// </summary>
        /// <returns>格式化的记忆文本，可直接返回给 AI；null 表示无结果或异常（调用方应降级）</returns>
        public async Task<string?> SearchMemoryVectorAsync(string keyword, long userId, int tenantId, long aiAppsId, string? memoryType = null)
        {
            if (!IsAvailable) return null;

            // ① 生成查询向量
            var queryEmbedding = await _ollamaApiService!.GetEmbedding(keyword);
            var collectionName = GetCollectionName(tenantId, aiAppsId, userId);

            if (!await IsCollectionExistsAsync(collectionName))
                return null;

            // ② Qdrant 相似度检索（集合已按 租户+智能体+用户 隔离，候选均为当前用户记忆）
            var searchResults = await Client.SearchAsync(
                collectionName,
                queryEmbedding.Vector,
                limit: SearchFetchLimit);

            // ③ 防御性校验：userId + tenantId + aiAppsId（集合已三维隔离，此处为双重保险）
            var candidates = FilterByOwnerScope(searchResults, userId, tenantId, aiAppsId);

            if (candidates.Count == 0)
                return null;

            // ④ 可选 Rerank 重排（提高语义相关性排序精度）
            if (_rerankService != null)
            {
                candidates = await RerankCandidatesAsync(keyword, candidates);
            }

            // ⑤ 按 memoryType 过滤 + 取 TopN
            if (!string.IsNullOrWhiteSpace(memoryType))
            {
                var validTypes = ParseMemoryTypes(memoryType);
                if (validTypes.Count > 0)
                {
                    candidates = candidates
                        .Where(c => validTypes.Contains(GetPayloadString(c, "memoryType")))
                        .ToList();
                }
            }

            var topResults = candidates.Take(MaxReturnCount).ToList();
            if (topResults.Count == 0)
                return null;

            return FormatSearchResults(keyword, topResults);
        }

        #endregion

        #region 向量删除

        /// <summary>
        /// 从 Qdrant 删除记忆向量（记忆被删除或软删除时调用）
        /// </summary>
        public async Task DeleteMemoryVectorAsync(long memoryId, int tenantId, long aiAppsId, long userId)
        {
            if (!IsAvailable) return;

            var collectionName = GetCollectionName(tenantId, aiAppsId, userId);
            if (!await IsCollectionExistsAsync(collectionName)) return;

            await Client.DeleteAsync(collectionName, new List<ulong> { (ulong)memoryId });
        }

        #endregion

        #region 私有方法 —— 集合管理

        /// <summary>
        /// 获取记忆集合名称（按 租户+智能体+用户 三维隔离，每个用户在每个智能体下拥有独立集合）
        /// </summary>
        private static string GetCollectionName(int tenantId, long aiAppsId, long userId)
            => $"{CollectionPrefix}_{tenantId}_{aiAppsId}_{userId}";

        /// <summary>
        /// 确保集合存在，不存在则自动创建（余弦距离）
        /// </summary>
        private async Task EnsureCollectionExistsAsync(string collectionName)
        {
            if (await IsCollectionExistsAsync(collectionName)) return;

            await Client.CreateCollectionAsync(collectionName, new VectorParams
            {
                Size = (ulong)_embeddingSize,
                Distance = Distance.Cosine
            });
        }

        /// <summary>
        /// 检查集合是否存在
        /// </summary>
        private async Task<bool> IsCollectionExistsAsync(string collectionName)
        {
            try
            {
                await Client.GetCollectionInfoAsync(collectionName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 私有方法 —— 搜索辅助

        /// <summary>
        /// 构建用于 Embedding 的文本（内容 + 关键词拼接，增强语义覆盖）
        /// </summary>
        private static string BuildEmbeddingText(string content, string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords)) return content;
            return $"{keywords} {content}";
        }

        /// <summary>
        /// 防御性校验：按 userId、tenantId、aiAppsId 筛选 Qdrant 搜索结果。
        /// 集合本身已按三维隔离，此处为双重保险，防止集合数据错位导致越权读取。
        /// </summary>
        private static List<ScoredPoint> FilterByOwnerScope(IEnumerable<ScoredPoint> results, long userId, int tenantId, long aiAppsId)
        {
            var userIdStr = userId.ToString();
            var tenantIdStr = tenantId.ToString();
            var aiAppsIdStr = aiAppsId.ToString();

            return results
                .Where(r => GetPayloadString(r, "userId") == userIdStr
                         && GetPayloadString(r, "tenantId") == tenantIdStr
                         && GetPayloadString(r, "aiAppsId") == aiAppsIdStr)
                .ToList();
        }

        /// <summary>
        /// 使用 Rerank 模型对候选结果重排序
        /// </summary>
        private async Task<List<ScoredPoint>> RerankCandidatesAsync(string query, List<ScoredPoint> candidates)
        {
            try
            {
                var documents = candidates.Select(c => GetPayloadString(c, "content")).ToList();
                var rerankResult = await _rerankService!.RerankAsync(query, documents, MaxReturnCount);

                if (rerankResult?.results == null || rerankResult.results.Count == 0)
                    return candidates;

                return rerankResult.results
                    .Where(r => r.index >= 0 && r.index < candidates.Count)
                    .OrderBy(r => r.index)
                    .Select(r => candidates[r.index])
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIQdrantMemory] Rerank 失败，使用原始排序: {ex.Message}");
                return candidates;
            }
        }

        /// <summary>
        /// 解析 memoryType 过滤参数（逗号分隔多类型）
        /// </summary>
        private static List<string> ParseMemoryTypes(string memoryType)
        {
            return memoryType
                .Split(new[] { ',', '，', '|', '、', ' ', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLowerInvariant())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// 安全获取 ScoredPoint 的 Payload 字符串值
        /// </summary>
        private static string GetPayloadString(ScoredPoint point, string key)
        {
            if (point.Payload == null || !point.Payload.ContainsKey(key))
                return string.Empty;
            return point.Payload[key].StringValue ?? string.Empty;
        }

        /// <summary>
        /// 将搜索结果格式化为 AI 可读的文本
        /// </summary>
        private static string FormatSearchResults(string keyword, List<ScoredPoint> results)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"共找到 {results.Count} 条语义相关记忆：");

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                var id = long.Parse(r.Id.Num.ToString());
                var memoryType = GetPayloadString(r, "memoryType");
                var importance = GetPayloadString(r, "importance");
                var content = GetPayloadString(r, "content");
                var keywords = GetPayloadString(r, "keywords");

                sb.AppendLine($"{i + 1}. [Id:{id}] [类型:{memoryType}] [重要度:{importance}] {content}");
                if (!string.IsNullOrWhiteSpace(keywords))
                {
                    sb.AppendLine($"   关键词：{keywords}");
                }
            }

            sb.AppendLine("如需更新或删除某条记忆，请使用对应的 Id。");
            return sb.ToString();
        }

        #endregion
    }
}
