using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                CreateUserId = CurrentUser.UserId,
                IsDelete = false,
                TenantId = CurrentUser.TenantId,
                ThreadId = t.ThreadId,
                Timestamp = t.Timestamp,
                Role = t.Role,
                Key = t.Key,
                SerializedMessage = t.SerializedMessage,
                MessageText = t.MessageText,
                MessageId = t.MessageId
            }).ToList();
            aIChatMessageStoreRp.AddRange(adddata);
            await aIChatMessageStoreRp.SaveChangesAsync();
        }

        public Task<List<ChatHistoryItemDto>> GetMessagesAsync(string threadId, CancellationToken cancellationToken)
        {
            return aIChatMessageStoreRp.Query().Where(t => t.ThreadId == threadId && t.IsDelete == false).Select(t => new ChatHistoryItemDto
            {
                Key = t.Key,
                ThreadId = t.ThreadId,
                Timestamp = t.Timestamp,
                SerializedMessage = t.SerializedMessage,
                MessageText = t.MessageText,
                Role = t.Role,
                MessageId = t.MessageId
            }).ToListAsync();
        }
    }
}
