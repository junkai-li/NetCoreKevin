using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.TextLimits;
using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.RepositorieRps.Repositories.AI;
using Microsoft.Extensions.AI;
using TencentCloud.Lke.V20231130.Models;

namespace kevin.Application.Services.AI
{
    public class KevinAIChatMessageStore : BaseService, IKevinAIChatMessageStore
    {

        public IAIChatMessageStoreRp aIChatMessageStoreRp { get; set; }
        public KevinAIChatMessageStore(IHttpContextAccessor _httpContextAccessor, IAIChatMessageStoreRp _aIChatMessageStoreRp) : base(_httpContextAccessor)
        {

            aIChatMessageStoreRp = _aIChatMessageStoreRp;
        }
        public async Task AddMessagesAsync(List<ChatHistoryItemDto> chatHistoryItems, CancellationToken cancellationToken)
        {
            var adddata = chatHistoryItems.Select(t => new TAIChatMessageStore
            {
                Id = SnowflakeIdService.GetNextId(),
                CreateTime = DateTime.Now,
                IsDelete = false,
                TenantId = CurrentUser.TenantId,
                ThreadId = t.ThreadId ?? "",
                Timestamp = t.Timestamp,
                Role = t.Role,
                Key = t.Key ?? "",
                SerializedMessage = t.SerializedMessage ?? "",
                MessageText = t.MessageText,
                MessageId = t.MessageId ?? SnowflakeIdService.GetNextId().ToString()
            }).ToList();

            // 按累计体积分批提交，避免一次长会话的工具消息把单条 INSERT 包撑爆：
            // AddRange 后的一次提交会被 EF 批处理成一条多值 INSERT 发给 MySQL，累计体积一旦超过
            // max_allowed_packet（默认 4MB）整批写入会直接失败（Broken pipe），上限取自 AIChatStorageSetting
            var maxCharsPerSave = CharLimits.MaxCharsPerSave;
            var batch = new List<TAIChatMessageStore>();
            var batchSize = 0;
            foreach (var item in adddata)
            {
                var size = (item.SerializedMessage?.Length ?? 0) + (item.MessageText?.Length ?? 0);
                if (batch.Count > 0 && batchSize + size > maxCharsPerSave)
                {
                    aIChatMessageStoreRp.AddRange(batch);
                    await aIChatMessageStoreRp.SaveChangesAsync(cancellationToken);
                    batch.Clear();
                    batchSize = 0;
                }
                batch.Add(item);
                batchSize += size;
            }
            if (batch.Count > 0)
            {
                aIChatMessageStoreRp.AddRange(batch);
                await aIChatMessageStoreRp.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<ChatHistoryItemDto>> GetMessagesAsync(string threadId, CancellationToken cancellationToken, int maxUserTurns = 0)
        {
            // 只回放未压缩消息：被压缩的旧消息已由压缩摘要替代，再次回放会与摘要重复占用上下文预算。
            // maxUserTurns 保留在签名中兼容调用方；“最近 N 轮保底”由 KevinChatMessageStore 在按 Token 裁剪时处理，
            // 这里不再按轮数硬截断，避免固定轮数架空 Token 预算（大文件/大工具结果的轮次体积差异极大）。
            return await aIChatMessageStoreRp.Query().Where(t => t.ThreadId == threadId && t.IsDelete == false && t.IsCompaction == false).Select(t => new ChatHistoryItemDto
            {
                Key = t.Key,
                ThreadId = t.ThreadId,
                Timestamp = t.Timestamp,
                SerializedMessage = t.SerializedMessage,
                MessageText = t.MessageText,
                Role = t.Role,
                MessageId = t.MessageId,
                CreateTime = t.CreateTime
            }).ToListAsync(cancellationToken);
        }
    }
}
