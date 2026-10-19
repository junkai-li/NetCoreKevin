using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 智能体记忆服务（用户级长期与短期记忆）
    /// <para>
    /// 搜索策略：优先通过 <see cref="AIQdrantAgentMemoryService"/> 进行 Qdrant 向量语义检索，
    /// 若 Qdrant 不可用或搜索无结果则降级到数据库关键词搜索。
    /// 写入/更新/删除操作以数据库为主，成功后异步同步向量到 Qdrant（失败不影响主流程）。
    /// </para>
    /// </summary>
    public class AIAgentMemoryService : BaseService, IAIAgentMemoryService
    {
        public IAIAgentMemoryRp AIAgentMemoryRp { get; set; }

        public IAIShareInfoService AIShareInfoService { get; set; }

        /// <summary>
        /// Qdrant 向量记忆服务（可选，为 null 时自动降级到数据库关键词搜索）
        /// </summary>
        public IAIQdrantAgentMemoryService? QdrantVectorService { get; set; }

        /// <summary>
        /// 数据库无精确匹配时 fallback 返回的最低重要度（9-10 核心约束 / 7-8 重要决策偏好）
        /// </summary>
        private const int FallbackMinImportance = 7;

        /// <summary>
        /// 单条记忆内容的最大长度（字符），超出直接拒绝而不是截断，避免静默丢信息
        /// </summary>
        private const int MaxContentLength = 4000;

        /// <summary>
        /// 关键词最大长度，与实体 TAIAgentMemory.Keywords 的 MaxLength(200) 保持一致
        /// </summary>
        private const int MaxKeywordsLength = 200;

        public AIAgentMemoryService(IHttpContextAccessor _httpContextAccessor, IAIAgentMemoryRp _AIAgentMemoryRp, IAIShareInfoService aIShareInfoService, IServiceProvider serviceProvider) : base(_httpContextAccessor)
        {
            this.AIAgentMemoryRp = _AIAgentMemoryRp;
            AIShareInfoService = aIShareInfoService;
            // 从服务容器解析 Qdrant 向量服务（可选依赖，未注册时为 null，自动降级到数据库搜索）。
            // HTTP 请求内优先用请求作用域；Hangfire 等无 HttpContext 场景回退到 DI 注入的容器。
            var provider = _serviceProvider ?? serviceProvider;
            QdrantVectorService = provider.GetService(typeof(IAIQdrantAgentMemoryService)) as IAIQdrantAgentMemoryService;
        }

        public async Task<dtoPageData<TAIAgentMemory>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAIAgentMemory>();
            var now = DateTime.Now;
            // 列表只展示未删除且未过期的记忆（过期短期记忆由清理任务软删除）
            var data = AIAgentMemoryRp.Query(isDataPer: true)
                .Where(t => t.IsDelete == false && (t.ExpireTime == null || t.ExpireTime > now));

            // searchKey 模糊匹配关键词或内容（原先该参数未生效）
            var keyword = dtoPagePar.searchKey?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                data = data.Where(t => t.Keywords.Contains(keyword) || t.Content.Contains(keyword));
            }

            result.total = await data.CountAsync();
            // 先排序再分页，保证跳页顺序稳定（原实现是先分页再排序）
            result.data = await data
                .OrderByDescending(x => x.CreateTime)
                .Skip(skip)
                .Take(dtoPagePar.pageSize)
                .ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        /// <summary>
        /// 保存记忆
        /// </summary>
        /// <remarks>
        /// 校验顺序：用户身份 → content 非空/长度 → memoryType 合法性 → importance 范围 → keywords 非空/长度 → expireTime 解析与类型匹配 → 去重检查 → 写入。
        /// task 是跨会话可复用的短期记忆，必须设置 expireTime；其他类型为长期记忆，禁止设置 expireTime。
        /// 去重规则：用 keywords 查询同用户/租户/智能体下已有记忆，若 content 高度相似（互相包含或前 30 字符相同）则拒绝并提示改用 UpdateMemory。
        /// </remarks>
        public async Task<string> SaveMemoryAsync(string content, string keywords, string memoryType, int importance, string expireTime = "")
        {
            if (AIShareInfoService.GetData().UserId <= 0)
            {
                return "❌ 保存记忆失败：无法获取当前用户，请在登录后使用。";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                return "❌ 保存记忆失败：记忆内容不能为空。";
            }
            if (content.Trim().Length > MaxContentLength)
            {
                return $"❌ 保存记忆失败：内容过长（{content.Trim().Length} 字符），请精简到 {MaxContentLength} 字符以内。";
            }
            // memoryType 合法性校验（7 种：preference/fact/task/decision/pitfall/skill/other）
            if (!MemoryTypes.IsValid(memoryType))
            {
                return $"❌ 保存记忆失败：memoryType “{memoryType}” 非法。合法值：{MemoryTypes.GetDescriptionText()}。";
            }
            var normalizedType = MemoryTypes.Normalize(memoryType);
            // importance 范围校验（0-10）
            if (importance < 0 || importance > 10)
            {
                return $"❌ 保存记忆失败：importance 必须在 0-10 之间，当前值 {importance}。评分参考：9-10 核心约束 / 7-8 重要决策偏好 / 5-6 一般事实经验 / 3-4 边缘信息 / 0-2 低价值。";
            }
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return "❌ 保存记忆失败：keywords 不能为空，请提供 2-5 个核心实体/概念/技术术语，英文逗号分隔。";
            }
            if (keywords.Trim().Length > MaxKeywordsLength)
            {
                return $"❌ 保存记忆失败：keywords 过长（{keywords.Trim().Length} 字符），请精简到 {MaxKeywordsLength} 字符以内（2-5 个核心词即可）。";
            }
            // task 是短期记忆，必须设置 expireTime；其他类型是长期记忆，禁止设置 expireTime。
            var hasExpireTime = !string.IsNullOrWhiteSpace(expireTime);
            if (normalizedType == MemoryTypes.Task && !hasExpireTime)
            {
                return "❌ 保存记忆失败：短期记忆（task）必须设置 expireTime。";
            }
            if (normalizedType != MemoryTypes.Task && hasExpireTime)
            {
                return $"❌ 保存记忆失败：{normalizedType} 是长期记忆，不允许设置 expireTime；只有短期记忆（task）可以设置过期时间。";
            }

            DateTime? parsedExpireTime = null;
            if (hasExpireTime)
            {
                string[] formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ssZ", "o" };
                if (!DateTime.TryParseExact(expireTime.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsed)
                    && !DateTime.TryParse(expireTime.Trim(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed))
                {
                    return $"❌ 保存记忆失败：expireTime 格式无法识别。支持格式：yyyy-MM-dd HH:mm，yyyy-MM-dd HH:mm:ss，ISO 8601。例如：2026-12-31 23:59。当前值：{expireTime}。";
                }
                parsedExpireTime = DateTime.SpecifyKind(parsed, DateTimeKind.Local);
                if (parsedExpireTime.Value <= DateTime.Now)
                {
                    return $"❌ 保存记忆失败：expireTime 必须大于当前时间。传入值：{parsedExpireTime.Value:yyyy-MM-dd HH:mm:ss}，当前时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}。";
                }
            }

            // 去重检查：用 keywords 查询同用户/租户/智能体下已有记忆，若 content 高度相似则拒绝
            var words = keywords
                .Split(new[] { ',', '，', '|', '、', ' ', '　' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
            if (words.Count > 0)
            {
                var now = DateTime.Now;
                var userId = AIShareInfoService.GetData().UserId;
                var tenantId = AIShareInfoService.GetData().TenantId;
                var aiAppsId = AIShareInfoService.GetData().AIAppsId;
                var keyPredicate = BuildKeywordPredicate(words);
                var existing = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                    .Where(t => t.IsDelete == false && t.UserId == userId && t.TenantId == tenantId && t.AIAppsId == aiAppsId && (t.ExpireTime == null || t.ExpireTime > now))
                    .Where(keyPredicate)
                    .OrderByDescending(t => t.Importance)
                    .ThenByDescending(t => t.CreateTime)
                    .Take(5)
                    .ToListAsync();
                var newContentTrim = content.Trim();
                foreach (var m in existing)
                {
                    if (IsContentHighlySimilar(m.Content, newContentTrim))
                    {
                        return $"⚠️ 已存在类似记忆（Id:{m.Id}, 类型:{m.MemoryType}, 重要度:{m.Importance}, 内容:{Truncate(m.Content, 80)}）。请改用 UpdateMemory 更新而非重复保存。";
                    }
                }
            }

            var add = new TAIAgentMemory();
            add.Id = SnowflakeIdService.GetNextId();
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.UserId = AIShareInfoService.GetData().UserId;
            add.AIAppsId = AIShareInfoService.GetData().AIAppsId;
            add.AIChatsId = AIShareInfoService.GetData().AIChatsId;
            add.TenantId = AIShareInfoService.GetData().TenantId;
            add.Content = content.Trim();
            add.Keywords = keywords.Trim();
            add.MemoryType = normalizedType;
            add.Importance = importance;
            add.ExpireTime = parsedExpireTime;
            AIAgentMemoryRp.Add(add);
            await AIAgentMemoryRp.SaveChangesAsync();

            // 同步向量到 Qdrant（失败不影响数据库主流程）
            await TrySyncVectorAsync(add);

            var expireInfo = parsedExpireTime.HasValue ? $", 过期时间：{parsedExpireTime.Value:yyyy-MM-dd HH:mm:ss}" : "";
            return $"✅ 记忆已保存（Id：{add.Id}, 类型：{normalizedType}, 重要度：{importance}{expireInfo}）。";
        }

        /// <summary>
        /// 判断两条记忆内容是否高度相似（互相包含 或 前 30 字符相同）
        /// </summary>
        private static bool IsContentHighlySimilar(string existing, string incoming)
        {
            if (string.IsNullOrWhiteSpace(existing) || string.IsNullOrWhiteSpace(incoming)) return false;
            var e = existing.Trim();
            var i = incoming.Trim();
            // 互相包含
            if (e.Contains(i, StringComparison.OrdinalIgnoreCase) || i.Contains(e, StringComparison.OrdinalIgnoreCase)) return true;
            // 前 30 字符相同
            var prefixLen = Math.Min(30, Math.Min(e.Length, i.Length));
            if (prefixLen >= 10 && string.Equals(e.Substring(0, prefixLen), i.Substring(0, prefixLen), StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// 截断字符串用于错误提示展示
        /// </summary>
        private static string Truncate(string value, int maxLen)
        {
            if (string.IsNullOrEmpty(value)) return value ?? "";
            return value.Length <= maxLen ? value : value.Substring(0, maxLen) + "...";
        }

        /// <summary>
        /// 搜索记忆
        /// </summary>
        /// <remarks>
        /// 支持按 memoryType 过滤（逗号分隔多类型），非法类型自动忽略不报错。
        /// </remarks>
        public async Task<string> SearchMemoryAsync(string keyword, string memoryType = "")
        {
            if (AIShareInfoService.GetData().UserId <= 0)
            {
                return "❌ 搜索记忆失败：无法获取当前用户，请在登录后使用。";
            }

            // ── 优先：Qdrant 向量语义检索（含可选 Rerank） ──
            if (QdrantVectorService?.IsAvailable == true)
            {
                try
                {
                    var vectorResult = await QdrantVectorService.SearchMemoryVectorAsync(
                        keyword,
                        AIShareInfoService.GetData().UserId,
                        AIShareInfoService.GetData().TenantId,
                        AIShareInfoService.GetData().AIAppsId,
                        memoryType);

                    if (!string.IsNullOrEmpty(vectorResult))
                        return vectorResult;
                    // 向量搜索无结果时继续降级到数据库关键词搜索
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AIAgentMemory] 向量搜索异常，降级到数据库搜索: {ex.Message}");
                }
            }

            // ── 降级：数据库关键词搜索 ──
            var words = (keyword ?? "")
                .Split(new[] { ',', '，', '|', '、', ' ', '　' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
            if (words.Count == 0)
            {
                return "❌ 搜索记忆失败：请提供检索关键词。";
            }
            var now = DateTime.Now;
            var query = AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.UserId == AIShareInfoService.GetData().UserId && t.TenantId == AIShareInfoService.GetData().TenantId && t.AIAppsId == AIShareInfoService.GetData().AIAppsId && (t.ExpireTime == null || t.ExpireTime > now));

            // 按 memoryType 过滤（支持逗号分隔多类型，非法类型自动忽略）
            if (!string.IsNullOrWhiteSpace(memoryType))
            {
                var types = memoryType
                    .Split(new[] { ',', '，', '|', '、', ' ', '　' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(t => t.ToLowerInvariant())
                    .Where(t => MemoryTypes.IsValid(t))
                    .Distinct()
                    .ToList();
                if (types.Count > 0)
                {
                    query = query.Where(t => types.Contains(t.MemoryType));
                }
            }

            var keyPredicate = BuildKeywordPredicate(words);
            var list = await query.Where(keyPredicate)
                .OrderByDescending(t => t.Importance)
                .ThenByDescending(t => t.CreateTime)
                .Take(10)
                .ToListAsync();
            if (list.Count == 0)
            {
                // Fallback：无精确匹配时仅返回高重要度记忆（≥7），供 AI 判断相关性；
                // 不再无差别返回最近记忆，避免用户记忆增多后把无关信息注入模型造成误用
                var fallback = await query
                    .Where(t => t.Importance >= FallbackMinImportance)
                    .OrderByDescending(t => t.Importance)
                    .ThenByDescending(t => t.CreateTime)
                    .Take(5)
                    .ToListAsync();
                if (fallback.Count == 0)
                {
                    return $"未找到与「{keyword}」相关的记忆。";
                }
                var sbFallback = new System.Text.StringBuilder();
                sbFallback.AppendLine($"未找到与「{keyword}」精确匹配的记忆。以下是当前用户的重要记忆（重要度≥{FallbackMinImportance}），请判断是否相关：");
                for (int i = 0; i < fallback.Count; i++)
                {
                    var m = fallback[i];
                    sbFallback.AppendLine($"{i + 1}. [Id:{m.Id}] [类型:{m.MemoryType}] [重要度:{m.Importance}] {m.Content}");
                    if (!string.IsNullOrWhiteSpace(m.Keywords))
                    {
                        sbFallback.AppendLine($"   关键词: {m.Keywords}");
                    }
                }
                return sbFallback.ToString();
            }
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"共找到 {list.Count} 条相关记忆：");
            for (int i = 0; i < list.Count; i++)
            {
                var m = list[i];
                sb.AppendLine($"{i + 1}. [Id:{m.Id}] [类型:{m.MemoryType}] [重要度:{m.Importance}] {m.Content}");
                if (!string.IsNullOrWhiteSpace(m.Keywords))
                {
                    sb.AppendLine($"   关键词：{m.Keywords}");
                }
            }
            sb.AppendLine("如需更新或删除某条记忆，请使用对应的 Id。");
            return sb.ToString();
        }

        /// <summary>
        /// 更新记忆
        /// </summary>
        public async Task<string> UpdateMemoryAsync(long id, string content, string keywords = "")
        {
            if (AIShareInfoService.GetData().UserId <= 0)
            {
                return "❌ 更新记忆失败：无法获取当前用户，请在登录后使用。";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                return "❌ 更新记忆失败：记忆内容不能为空。";
            }
            if (content.Trim().Length > MaxContentLength)
            {
                return $"❌ 更新记忆失败：内容过长（{content.Trim().Length} 字符），请精简到 {MaxContentLength} 字符以内。";
            }
            if (!string.IsNullOrWhiteSpace(keywords) && keywords.Trim().Length > MaxKeywordsLength)
            {
                return $"❌ 更新记忆失败：keywords 过长（{keywords.Trim().Length} 字符），请精简到 {MaxKeywordsLength} 字符以内（2-5 个核心词即可）。";
            }
            // 归属校验与搜索/保存保持一致：租户+智能体+用户三维，防止跨智能体操作自己的记忆
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id
                    && t.UserId == AIShareInfoService.GetData().UserId
                    && t.TenantId == AIShareInfoService.GetData().TenantId
                    && t.AIAppsId == AIShareInfoService.GetData().AIAppsId)
                .FirstOrDefaultAsync();
            if (data == default)
            {
                return $"❌ 更新记忆失败：未找到 Id 为 {id} 的记忆，或无权操作。请先搜索记忆获取正确的 Id。";
            }
            data.Content = content.Trim();
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                data.Keywords = keywords.Trim();
            }
            data.UpdateTime = DateTime.Now;
            await AIAgentMemoryRp.SaveChangesAsync();

            // 同步向量到 Qdrant（内容/关键词变更后需更新向量）
            await TrySyncVectorAsync(data);

            return $"✅ 记忆已更新（Id：{id}）。";
        }

        /// <summary>
        /// 删除记忆
        /// </summary>
        public async Task<string> DeleteMemoryAsync(long id)
        {
            if (AIShareInfoService.GetData().UserId <= 0)
            {
                return "❌ 删除记忆失败：无法获取当前用户，请在登录后使用。";
            }
            // 归属校验与搜索/保存保持一致：租户+智能体+用户三维，防止跨智能体操作自己的记忆
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id
                    && t.UserId == AIShareInfoService.GetData().UserId
                    && t.TenantId == AIShareInfoService.GetData().TenantId
                    && t.AIAppsId == AIShareInfoService.GetData().AIAppsId)
                .FirstOrDefaultAsync();
            if (data == default)
            {
                return $"❌ 删除记忆失败：未找到 Id 为 {id} 的记忆，或无权操作。请先搜索记忆获取正确的 Id。";
            }
            data.IsDelete = true;
            data.DeleteTime = DateTime.Now;
            await AIAgentMemoryRp.SaveChangesAsync();

            // 同步删除 Qdrant 中的向量
            await TryDeleteVectorAsync(data);

            return $"✅ 记忆已删除（Id：{id}）。";
        }

        public async Task<bool> AddEdit(TAIAgentMemory data)
        {
            if (!string.IsNullOrWhiteSpace(data.Content) && data.Content.Trim().Length > MaxContentLength)
            {
                throw new UserFriendlyException($"记忆内容过长，最多 {MaxContentLength} 字符");
            }
            if (!string.IsNullOrWhiteSpace(data.Keywords) && data.Keywords.Trim().Length > MaxKeywordsLength)
            {
                throw new UserFriendlyException($"记忆关键词过长，最多 {MaxKeywordsLength} 字符");
            }
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AIAgentMemoryRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            TAIAgentMemory affected;
            if (isAdd)
            {
                var add = data.MapTo<TAIAgentMemory>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.AIChatsId = AIShareInfoService.GetData().AIChatsId;
                add.AIAppsId = AIShareInfoService.GetData().AIAppsId;
                // 归属用户兜底：管理端未显式指定 UserId 时取当前登录用户，避免 UserId=0 的记忆任何用户都检索不到
                if (add.UserId <= 0)
                {
                    add.UserId = AIShareInfoService.GetData().UserId;
                }
                AIAgentMemoryRp.Add(add);
                affected = add;
            }
            else
            {
                var upData = AIAgentMemoryRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    var originalUserId = upData.UserId;
                    var originalTenantId = upData.TenantId;
                    upData = data.MapTo(upData);
                    // 编辑时前端未传归属字段会把 UserId/TenantId 覆盖为 0，这里保留原值防止记忆失去归属
                    if (upData.UserId <= 0) upData.UserId = originalUserId;
                    if (upData.TenantId <= 0) upData.TenantId = originalTenantId;
                    upData.UpdateTime = DateTime.Now;
                    affected = upData;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AIAgentMemoryRp.SaveChangesAsync();

            // 同步向量到 Qdrant（新增或内容变更后更新向量，失败静默降级不影响数据库主流程）
            await TrySyncVectorAsync(affected);
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIAgentMemoryRp.SaveChangesWithSaveLog();

                // 同步删除 Qdrant 向量，避免已删除记忆仍被语义搜索命中（失败静默降级不影响数据库主流程）
                await TryDeleteVectorAsync(data);
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        /// <summary>
        /// 清理已过期的短期记忆（由 Hangfire 定时任务调用）。
        /// <para>
        /// 过期记忆在检索时已被过滤，但仍会占用数据库与向量存储；这里按批软删除并同步删除 Qdrant 向量。
        /// 操作幂等，可重复执行。
        /// </para>
        /// </summary>
        public async Task CleanupExpiredMemoriesAsync()
        {
            const int batchSize = 500;
            var now = DateTime.Now;
            var expired = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.ExpireTime != null && t.ExpireTime <= now)
                .OrderBy(t => t.ExpireTime)
                .Take(batchSize)
                .ToListAsync();
            if (expired.Count == 0)
            {
                return;
            }

            foreach (var memory in expired)
            {
                memory.IsDelete = true;
                memory.DeleteTime = now;
            }
            await AIAgentMemoryRp.SaveChangesAsync();

            // 同步删除 Qdrant 向量（失败静默降级，残留向量会被检索时的过期过滤拦截）
            foreach (var memory in expired)
            {
                await TryDeleteVectorAsync(memory);
            }
        }

        #region 向量同步辅助方法（Qdrant 降级保护）

        /// <summary>
        /// 尝试将记忆向量同步到 Qdrant，失败时静默降级（不影响数据库主流程）
        /// </summary>
        private async Task TrySyncVectorAsync(TAIAgentMemory memory)
        {
            if (QdrantVectorService?.IsAvailable != true) return;
            try
            {
                await QdrantVectorService.UpsertMemoryVectorAsync(memory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIAgentMemory] Qdrant 向量同步失败（记忆Id：{memory.Id}），已降级到数据库模式: {ex.Message}");
            }
        }

        /// <summary>
        /// 尝试从 Qdrant 删除记忆向量，失败时静默降级
        /// </summary>
        private async Task TryDeleteVectorAsync(TAIAgentMemory memory)
        {
            if (QdrantVectorService?.IsAvailable != true) return;
            try
            {
                await QdrantVectorService.DeleteMemoryVectorAsync(memory.Id, memory.TenantId, memory.AIAppsId, memory.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIAgentMemory] Qdrant 向量删除失败（记忆Id：{memory.Id}），已降级到数据库模式: {ex.Message}");
            }
        }

        #endregion

        /// <summary>
        /// 构建关键词匹配表达式：任一关键词命中 Keywords 或 Content 即匹配（OR）
        /// </summary>
        private static Expression<Func<TAIAgentMemory, bool>> BuildKeywordPredicate(List<string> words)
        {
            var param = Expression.Parameter(typeof(TAIAgentMemory), "t");
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            var keywordsProp = Expression.Property(param, nameof(TAIAgentMemory.Keywords));
            var contentProp = Expression.Property(param, nameof(TAIAgentMemory.Content));
            Expression? body = null;
            foreach (var word in words)
            {
                var constant = Expression.Constant(word);
                var hit = Expression.OrElse(
                    Expression.Call(keywordsProp, containsMethod, constant),
                    Expression.Call(contentProp, containsMethod, constant));
                body = body == null ? hit : Expression.OrElse(body, hit);
            }
            return Expression.Lambda<Func<TAIAgentMemory, bool>>(body!, param);
        }
    }
}
