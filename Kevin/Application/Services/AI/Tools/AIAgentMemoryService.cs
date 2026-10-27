using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.TextLimits;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json;

namespace kevin.Application.Services.AI.Tools
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
        /// 自动召回：注入上下文的最大条数（过多会挤占 token 并引入噪声）
        /// </summary>
        private const int AutoRecallTopN = 3;

        /// <summary>
        /// 自动召回：Qdrant 相似度阈值（高于工具搜索的 0.4，宁可不召回也不注入无关记忆）
        /// </summary>
        private const float AutoRecallMinScore = 0.55f;

        /// <summary>
        /// 自动召回：去掉空白/标点后的有效字符数低于该值（“嗯”“好”“ok”）不检索，避免无意义开销。
        /// 这是快速过滤下界，中文 2 字、英文 2 位即可承载信息，不再用统一的 4 字门槛
        /// </summary>
        private const int AutoRecallMinQueryLength = 2;

        /// <summary>
        /// 自动召回：有效字符数低于该值的算短查询。短查询默认不召回，
        /// 只有命中记忆触发词（<see cref="AutoRecallCueWords"/>）或含英文/数字词（Go、AI、NY、v2）时才放行
        /// </summary>
        private const int AutoRecallShortQueryLength = 6;

        /// <summary>
        /// 自动召回：纯寒暄、确认、继续类输入（按有效字符精确匹配）直接跳过。
        /// 用词表而不是长度排除，避免误杀“怕冷”“用Go”这类短但含信息的提问
        /// </summary>
        private static readonly string[] AutoRecallFillerQueries =
        {
            "嗯", "嗯嗯", "哦", "哦哦", "啊", "好", "好的", "好滴", "好吧", "行", "行吧", "可以",
            "继续", "对的", "是的", "是", "收到", "明白", "了解了", "谢谢", "多谢", "哈哈", "在吗", "你好", "早",
            "ok", "okay", "k", "yes", "no", "hi", "hello", "thanks", "thx"
        };

        /// <summary>
        /// 自动召回：短查询放行的记忆触发词。
        /// 覆盖回忆意图（之前/上次/记得）、删除与纠正意图（别记/忘记/删除/不对）、
        /// 第一人称稳定陈述（我/用/怕/住）和偏好/项目类话题——出现这些词时短句也可能承载关键信息
        /// </summary>
        private static readonly string[] AutoRecallCueWords =
        {
            "之前", "上次", "上回", "以前", "刚才", "刚刚", "还是按",
            "记得", "记住", "别忘了", "别记", "不要记", "不用记", "忘记", "忘掉",
            "删除", "删掉", "更正", "纠正", "我说过", "不是这样", "不对",
            "我的", "我有", "偏好", "喜欢", "讨厌", "习惯", "项目", "以后", "将来",
            "我", "怕", "用", "住"
        };

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
            if (content.Trim().Length > CharLimits.MemoryContent)
            {
                return $"❌ 保存记忆失败：内容过长（{content.Trim().Length} 字符），请精简到 {CharLimits.MemoryContent} 字符以内。";
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
            if (keywords.Trim().Length > CharLimits.MemoryKeywords)
            {
                return $"❌ 保存记忆失败：keywords 过长（{keywords.Trim().Length} 字符），请精简到 {CharLimits.MemoryKeywords} 字符以内（2-5 个核心词即可）。";
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
                    .ThenByDescending(t => t.HitCount)
                    .ThenByDescending(t => t.CreateTime)
                    .Take(5)
                    .ToListAsync();
                var newContentTrim = content.Trim();
                foreach (var m in existing)
                {
                    if (IsContentHighlySimilar(m.Content, newContentTrim))
                    {
                        return $"⚠️ 已存在类似记忆（Id:{m.Id}, 类型:{m.MemoryType}, 重要度:{m.Importance}, 内容:{CharLimits.Truncate(m.Content, CharLimits.MemoryDuplicatePreview, "...")}）。请改用 UpdateMemory 更新而非重复保存。";
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
                .ThenByDescending(t => t.HitCount)
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
                    .ThenByDescending(t => t.HitCount)
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
        /// 自动召回：按当前提问检索高相关记忆并格式化为可直接注入系统提示词的文本块。
        /// <para>
        /// 只在 Qdrant 可用时执行（数据库关键词搜索依赖模型给关键词，无法处理整句提问）；
        /// 召回门槛见 <see cref="ShouldAutoRecall"/>：寒暄/确认类输入直接跳过，短查询须命中记忆触发词
        /// 或含英文/数字词才放行。该门槛只限制自动召回，显式 SearchMemory 与保存前查重不受影响。
        /// 命中后累加 HitCount/LastHitTime 作为高频经验信号；任何异常都降级为空字符串，不影响主流程。
        /// </para>
        /// </summary>
        public async Task<string> BuildAutoRecallContextAsync(string query)
        {
            var share = AIShareInfoService.GetData();
            if (share.UserId <= 0) return "";
            if (QdrantVectorService?.IsAvailable != true) return "";

            var q = (query ?? "").Trim();
            if (!ShouldAutoRecall(q)) return "";
            q = CharLimits.Truncate(q, CharLimits.AutoRecallQuery, string.Empty);

            List<AIQdrantMemoryHit> hits;
            try
            {
                hits = await QdrantVectorService.SearchMemoryVectorHitsAsync(q, share.UserId, share.TenantId, share.AIAppsId, AutoRecallTopN, AutoRecallMinScore);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIAgentMemory] 自动召回失败，本轮跳过: {ex.Message}");
                return "";
            }
            if (hits.Count == 0) return "";

            var hitCounts = await TouchHitsAsync(hits.Select(t => t.Id).ToList());

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("## 自动召回的用户记忆");
            sb.AppendLine("以下记忆由系统按当前提问自动召回，可能不完全相关：");
            for (int i = 0; i < hits.Count; i++)
            {
                var h = hits[i];
                var hitInfo = hitCounts.TryGetValue(h.Id, out var count) ? $" [已命中:{count}]" : "";
                sb.AppendLine($"{i + 1}. [Id:{h.Id}] [类型:{h.MemoryType}] [重要度:{h.Importance}]{hitInfo} {CharLimits.Truncate(h.Content, CharLimits.AutoRecallContent, "...")}");
            }
            sb.AppendLine("使用规则：与本次任务相关的记忆必须遵守；与当前代码、配置、工具结果或用户当前指令冲突时以当前事实为准，并立即用 UpdateMemory 更正或 DeleteMemory 删除；不相关则忽略。不要向用户提及自动召回过程。");
            return sb.ToString();
        }

        /// <summary>
        /// 判断本轮提问是否值得自动召回：
        /// 有效字符过短或纯寒暄/确认输入（嗯、好、继续）直接跳过；
        /// 正常长度的提问直接召回；短查询只有在命中记忆触发词或含英文/数字词时才放行。
        /// 该门槛只作用于自动召回，不限制显式 SearchMemory 与保存前查重。
        /// </summary>
        private static bool ShouldAutoRecall(string query)
        {
            var compact = CompactQuery(query);
            if (compact.Length < AutoRecallMinQueryLength) return false;
            if (IsFillerQuery(compact)) return false;
            if (compact.Length >= AutoRecallShortQueryLength) return true;
            return ContainsCueWord(compact) || ContainsLatinOrDigitToken(compact);
        }

        /// <summary>
        /// 去掉空白、标点、符号与控制/代理字符，只保留有信息量的内容（“嗯...”归一为“嗯”）
        /// </summary>
        private static string CompactQuery(string query)
        {
            var sb = new System.Text.StringBuilder(query.Length);
            foreach (var c in query)
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c) || char.IsControl(c) || char.IsSurrogate(c)) continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsFillerQuery(string compact)
        {
            foreach (var filler in AutoRecallFillerQueries)
            {
                if (string.Equals(compact, filler, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static bool ContainsCueWord(string compact)
        {
            foreach (var cue in AutoRecallCueWords)
            {
                if (compact.Contains(cue, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// 是否含英文/数字词（两位以上，如 Go、AI、NY、v2）：短查询里的语言名、缩写、版本号往往本身就是关键信息
        /// </summary>
        private static bool ContainsLatinOrDigitToken(string compact)
        {
            var run = 0;
            foreach (var c in compact)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    run++;
                    if (run >= 2) return true;
                }
                else
                {
                    run = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// 累加自动召回命中次数（最近一次命中时间同步更新）。
        /// 计数仅用于识别高频经验，失败只记日志，不影响本轮回答。
        /// </summary>
        private async Task<Dictionary<long, int>> TouchHitsAsync(List<long> ids)
        {
            var result = new Dictionary<long, int>();
            if (ids.Count == 0) return result;
            try
            {
                var share = AIShareInfoService.GetData();
                var list = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                    .Where(t => t.IsDelete == false && ids.Contains(t.Id)
                        && t.UserId == share.UserId && t.TenantId == share.TenantId && t.AIAppsId == share.AIAppsId)
                    .ToListAsync();
                if (list.Count == 0) return result;

                var now = DateTime.Now;
                foreach (var memory in list)
                {
                    memory.HitCount += 1;
                    memory.LastHitTime = now;
                    result[memory.Id] = memory.HitCount;
                }
                await AIAgentMemoryRp.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIAgentMemory] 召回命中计数更新失败（不影响召回结果）: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 更新记忆
        /// </summary>
        public async Task<string> UpdateMemoryAsync(long id, string content, string keywords = "", int importance = -1)
        {
            if (AIShareInfoService.GetData().UserId <= 0)
            {
                return "❌ 更新记忆失败：无法获取当前用户，请在登录后使用。";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                return "❌ 更新记忆失败：记忆内容不能为空。";
            }
            if (content.Trim().Length > CharLimits.MemoryContent)
            {
                return $"❌ 更新记忆失败：内容过长（{content.Trim().Length} 字符），请精简到 {CharLimits.MemoryContent} 字符以内。";
            }
            if (!string.IsNullOrWhiteSpace(keywords) && keywords.Trim().Length > CharLimits.MemoryKeywords)
            {
                return $"❌ 更新记忆失败：keywords 过长（{keywords.Trim().Length} 字符），请精简到 {CharLimits.MemoryKeywords} 字符以内（2-5 个核心词即可）。";
            }
            // importance：-1 表示不修改；其余按 0-10 校验
            if (importance < -1 || importance > 10)
            {
                return $"❌ 更新记忆失败：importance 必须在 0-10 之间（传 -1 表示不修改），当前值 {importance}。";
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
            if (importance >= 0)
            {
                data.Importance = importance;
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
            if (!string.IsNullOrWhiteSpace(data.Content) && data.Content.Trim().Length > CharLimits.MemoryContent)
            {
                throw new UserFriendlyException($"记忆内容过长，最多 {CharLimits.MemoryContent} 字符");
            }
            if (!string.IsNullOrWhiteSpace(data.Keywords) && data.Keywords.Trim().Length > CharLimits.MemoryKeywords)
            {
                throw new UserFriendlyException($"记忆关键词过长，最多 {CharLimits.MemoryKeywords} 字符");
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
                    var originalHitCount = upData.HitCount;
                    var originalLastHitTime = upData.LastHitTime;
                    upData = data.MapTo(upData);
                    // 编辑时前端未传归属字段会把 UserId/TenantId 覆盖为 0，这里保留原值防止记忆失去归属
                    if (upData.UserId <= 0) upData.UserId = originalUserId;
                    if (upData.TenantId <= 0) upData.TenantId = originalTenantId;
                    // 命中次数由自动召回维护，管理端编辑不感知这两个字段，映射后回填防止被清零
                    upData.HitCount = originalHitCount;
                    upData.LastHitTime = originalLastHitTime;
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
