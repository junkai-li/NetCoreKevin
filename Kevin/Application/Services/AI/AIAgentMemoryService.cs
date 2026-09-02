using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 智能体记忆服务（用户级长期记忆）
    /// </summary>
    public class AIAgentMemoryService : BaseService, IAIAgentMemoryService
    {
        public IAIAgentMemoryRp AIAgentMemoryRp { get; set; }

        public IAIShareInfoService AIShareInfoService { get; set; }
         
        public AIAgentMemoryService(IHttpContextAccessor _httpContextAccessor, IAIAgentMemoryRp _AIAgentMemoryRp, IAIShareInfoService aIShareInfoService) : base(_httpContextAccessor)
        {
            this.AIAgentMemoryRp = _AIAgentMemoryRp;
            AIShareInfoService = aIShareInfoService;
        }

        public async Task<dtoPageData<TAIAgentMemory>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAIAgentMemory>();
            var data = AIAgentMemoryRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        /// <summary>
        /// 保存记忆
        /// </summary>
        /// <remarks>
        /// 校验顺序：用户身份 → content 非空 → memoryType 合法性 → importance 范围 → expireTime 解析与有效性 → 去重检查 → 写入。
        /// expireTime 为字符串（空表示永久有效），支持多格式解析：yyyy-MM-dd HH:mm、yyyy-MM-dd HH:mm:ss、ISO 8601，参考 KevinAITasksService.AddOnceTask 模式。
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
            // memoryType 合法性校验（7 种：preference/fact/task/decision/pitfall/skill/other）
            if (!MemoryTypes.IsValid(memoryType))
            {
                return $"❌ 保存记忆失败：memoryType “{memoryType}” 非法。合法值：{MemoryTypes.GetDescriptionText()}。详见系统提示词 4.3 分类表。";
            }
            // importance 范围校验（0-10）
            if (importance < 0 || importance > 10)
            {
                return $"❌ 保存记忆失败：importance 必须在 0-10 之间，当前值 {importance}。详见系统提示词 4.4 打分表。";
            }
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return "❌ 保存记忆失败：keywords 不能为空，请提供 2-5 个核心实体/概念/技术术语，英文逗号分隔。";
            }
            // expireTime 解析与校验：空字符串表示永久有效（null），支持多格式解析
            DateTime? parsedExpireTime = null;
            if (!string.IsNullOrWhiteSpace(expireTime))
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
            var normalizedType = MemoryTypes.Normalize(memoryType);
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
                // Fallback：无精确匹配时返回用户最近的记忆（按 importance + createTime），供 AI 自行判断相关性
                var fallback = await query
                    .OrderByDescending(t => t.Importance)
                    .ThenByDescending(t => t.CreateTime)
                    .Take(5)
                    .ToListAsync();
                if (fallback.Count == 0)
                {
                    return $"未找到与「{keyword}」相关的记忆，且当前用户无任何历史记忆。";
                }
                var sbFallback = new System.Text.StringBuilder();
                sbFallback.AppendLine($"未找到与「{keyword}」精确匹配的记忆。以下是当前用户最近的历史记忆，请判断是否相关：");
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
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id && t.UserId == AIShareInfoService.GetData().UserId)
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
            AIAgentMemoryRp.SaveChanges();
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
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id && t.UserId == AIShareInfoService.GetData().UserId)
                .FirstOrDefaultAsync();
            if (data == default)
            {
                return $"❌ 删除记忆失败：未找到 Id 为 {id} 的记忆，或无权操作。请先搜索记忆获取正确的 Id。";
            }
            data.IsDelete = true;
            data.DeleteTime = DateTime.Now;
            AIAgentMemoryRp.SaveChangesWithSaveLog();
            return $"✅ 记忆已删除（Id：{id}）。";
        }

        public async Task<bool> AddEdit(TAIAgentMemory data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AIAgentMemoryRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAIAgentMemory>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.AIChatsId = AIShareInfoService.GetData().AIChatsId;
                add.AIAppsId = AIShareInfoService.GetData().AIAppsId;
                AIAgentMemoryRp.Add(add);
            }
            else
            {
                var upData = AIAgentMemoryRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData = data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AIAgentMemoryRp.SaveChangesAsync();
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
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

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
