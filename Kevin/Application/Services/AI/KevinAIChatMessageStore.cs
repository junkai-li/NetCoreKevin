using kevin.AI.AgentFramework.Dto;
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

            aIChatMessageStoreRp.AddRange(adddata);
            await aIChatMessageStoreRp.SaveChangesAsync();
        }

        public async Task<List<ChatHistoryItemDto>> GetMessagesAsync(string threadId, CancellationToken cancellationToken, int maxUserTurns = 0)
        {
            if (maxUserTurns == 0)
            {
                return await aIChatMessageStoreRp.Query().Where(t => t.ThreadId == threadId && t.IsDelete == false).Select(t => new ChatHistoryItemDto
                {
                    Key = t.Key,
                    ThreadId = t.ThreadId,
                    Timestamp = t.Timestamp,
                    SerializedMessage = t.SerializedMessage,
                    MessageText = t.MessageText,
                    Role = t.Role,
                    MessageId = t.MessageId,
                    CreateTime=t.CreateTime
                }).ToListAsync();
            }
            else
            {
                var data = await aIChatMessageStoreRp.Query().Where(t => t.ThreadId == threadId && t.IsDelete == false).Select(t => new ChatHistoryItemDto
                {
                    Key = t.Key,
                    ThreadId = t.ThreadId,
                    Timestamp = t.Timestamp,
                    SerializedMessage = t.SerializedMessage,
                    MessageText = t.MessageText,
                    Role = t.Role,
                    MessageId = t.MessageId,
                    CreateTime = t.CreateTime
                }).OrderByDescending(t => t.Timestamp).ToListAsync();
                var reslutData = new List<ChatHistoryItemDto>();
                int userTurns = 0;
                foreach (var item in data)
                {
                    if (userTurns < maxUserTurns)
                    {
                        reslutData.Add(item);
                        if (item.Role == ChatRole.User.Value)
                        {
                            userTurns++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return reslutData;
            }

        }
    }
}
