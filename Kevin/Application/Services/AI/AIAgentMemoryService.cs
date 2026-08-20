using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
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

        private object? _data { get; set; }
        private long UserId = 0;
        private long AIChatsId = 0;
        private long AppId = 0;
        private int TenantId = 0;

        public void InitData(object data)
        {
            _data = data;
            if (_data != default)
            {
                var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                if (jsonDoc.RootElement.TryGetProperty("UserId", out var userIdEl))
                {
                    userIdEl.TryGetInt64(out UserId);
                }
                if (jsonDoc.RootElement.TryGetProperty("AIChatsId", out var chatsEl))
                {
                    chatsEl.TryGetInt64(out AIChatsId);
                }
                if (jsonDoc.RootElement.TryGetProperty("AppId", out var appEl))
                {
                    appEl.TryGetInt64(out AppId);
                }
                if (jsonDoc.RootElement.TryGetProperty("TenantId", out var tenantEl))
                {
                    tenantEl.TryGetInt32(out TenantId);
                }
            }
        }

        public AIAgentMemoryService(IHttpContextAccessor _httpContextAccessor, IAIAgentMemoryRp _AIAgentMemoryRp) : base(_httpContextAccessor)
        {
            this.AIAgentMemoryRp = _AIAgentMemoryRp;
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
        public async Task<string> SaveMemoryAsync(string content, string keywords, string memoryType = "other", int importance = 5)
        {
            if (UserId <= 0)
            {
                return "❌ 保存记忆失败：无法获取当前用户，请在登录后使用。";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                return "❌ 保存记忆失败：记忆内容不能为空。";
            }
            var add = new TAIAgentMemory();
            add.Id = SnowflakeIdService.GetNextId();
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.UserId = UserId;
            add.AIAppsId = AppId;
            add.AIChatsId = AIChatsId;
            add.TenantId = TenantId;
            add.Content = content.Trim();
            add.Keywords = (keywords ?? "").Trim();
            add.MemoryType = string.IsNullOrWhiteSpace(memoryType) ? "other" : memoryType.Trim();
            add.Importance = Math.Clamp(importance, 0, 10);
            AIAgentMemoryRp.Add(add);
            await AIAgentMemoryRp.SaveChangesAsync();
            return $"✅ 记忆已保存（Id：{add.Id}）。";
        }

        /// <summary>
        /// 搜索记忆
        /// </summary>
        public async Task<string> SearchMemoryAsync(string keyword)
        {
            if (UserId <= 0)
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
                .Where(t => t.IsDelete == false && t.UserId == UserId && t.TenantId == TenantId && t.AIAppsId == AppId && (t.ExpireTime == null || t.ExpireTime > now));
            var keyPredicate = BuildKeywordPredicate(words);
            var list = await query.Where(keyPredicate)
                .OrderByDescending(t => t.Importance)
                .ThenByDescending(t => t.CreateTime)
                .Take(10)
                .ToListAsync();
            if (list.Count == 0)
            {
                return $"未找到与「{keyword}」相关的记忆。";
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
            if (UserId <= 0)
            {
                return "❌ 更新记忆失败：无法获取当前用户，请在登录后使用。";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                return "❌ 更新记忆失败：记忆内容不能为空。";
            }
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id && t.UserId == UserId)
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
            if (UserId <= 0)
            {
                return "❌ 删除记忆失败：无法获取当前用户，请在登录后使用。";
            }
            var data = await AIAgentMemoryRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.Id == id && t.UserId == UserId)
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
                add.AIChatsId = AIChatsId;
                add.AIAppsId = AppId;
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
