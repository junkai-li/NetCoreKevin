using kevin.Application.Services.AI;
using kevin.Domain.Entities.AI;
using kevin.Domain.Events;
using Kevin.log4Net;
using Kevin.SnowflakeId.Service;
using MediatR;
using Repository.Database;

namespace kevin.Application.EventHandlers
{
    /// <summary>
    /// 聊天历史创建事件处理器：按聊天历史记录 Id 记录本轮 Token 使用明细，并累加当月已用额度。
    /// <para>
    /// 领域事件由 <see cref="KevinDbContext.SaveChangesAsync(bool, CancellationToken)"/> 在落库前发布且框架未 await
    /// （fire-and-forget），因此这里必须使用独立的 DbContext，不能复用请求作用域的 DbContext，
    /// 否则会与原请求的 SaveChanges 并发操作同一个 DbContext 导致异常。
    /// </para>
    /// </summary>
    public class TAIChatHistorysCreatedEventHandler : INotificationHandler<TAIChatHistorysCreatedEvent>
    {
        /// <summary>
        /// 并发冲突（乐观锁/唯一键）时的最大重试次数
        /// </summary>
        private const int MaxRetryCount = 3;

        private readonly ISnowflakeIdService _snowflakeIdService;

        public TAIChatHistorysCreatedEventHandler(ISnowflakeIdService snowflakeIdService)
        {
            _snowflakeIdService = snowflakeIdService;
        }

        public async Task Handle(TAIChatHistorysCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var chat = notification.Value;
                var userId = chat.CreateUserId;
                var inputTokenCount = chat.InputTokenCount ?? 0;
                var outputTokenCount = chat.OutputTokenCount ?? 0;
                var totalTokenCount = inputTokenCount + outputTokenCount;
                // 没有用户或模型未返回 token 用量时不记账
                if (userId <= 0 || totalTokenCount <= 0)
                {
                    return;
                }
                var quotaMonth = AITokenQuotaService.GetQuotaMonth(chat.CreateTime);

                for (var retry = 0; retry < MaxRetryCount; retry++)
                {
                    try
                    {
                        using var db = new KevinDbContext();
                        // 同一条聊天记录只记一次明细（事件重复发布保护）
                        var exists = await db.Set<TAIUserTokenUsage>()
                            .AnyAsync(t => t.IsDelete == false && t.AIChatHistorysId == chat.Id, CancellationToken.None);
                        if (exists)
                        {
                            return;
                        }

                        db.Set<TAIUserTokenUsage>().Add(new TAIUserTokenUsage
                        {
                            Id = _snowflakeIdService.GetNextId(),
                            UserId = userId,
                            QuotaMonth = quotaMonth,
                            AIChatHistorysId = chat.Id,
                            InputTokenCount = inputTokenCount,
                            OutputTokenCount = outputTokenCount,
                            TotalTokenCount = totalTokenCount,
                            IsDelete = false,
                            CreateTime = DateTime.Now,
                            TenantId = chat.TenantId
                        });

                        var quota = await db.Set<TAIUserTokenQuota>()
                            .FirstOrDefaultAsync(t => t.IsDelete == false && t.UserId == userId && t.TenantId == chat.TenantId && t.QuotaMonth == quotaMonth, CancellationToken.None);
                        if (quota == default)
                        {
                            // 月初任务尚未执行（或新用户当月首次对话）时兜底创建当月额度，避免用量丢失
                            db.Set<TAIUserTokenQuota>().Add(new TAIUserTokenQuota
                            {
                                Id = _snowflakeIdService.GetNextId(),
                                UserId = userId,
                                QuotaMonth = quotaMonth,
                                TotalQuota = AITokenQuotaService.DefaultMonthlyQuota,
                                UsedQuota = totalTokenCount,
                                IsDelete = false,
                                CreateTime = DateTime.Now,
                                TenantId = chat.TenantId
                            });
                        }
                        else
                        {
                            quota.UsedQuota += totalTokenCount;
                        }

                        await db.SaveChangesAsync(CancellationToken.None);
                        return;
                    }
                    catch (DbUpdateException ex)
                    {
                        // 同一用户多轮对话并发完成时，已用额度会撞乐观锁或当月额度唯一键，重新读取最新值再累加
                        LogHelper.logger.Info($"Token 额度记录并发冲突，第 {retry + 1} 次重试：聊天记录 {chat.Id}，{ex.Message}");
                    }
                }
                LogHelper.logger.Error($"Token 使用明细记录失败：聊天记录 {chat.Id} 连续 {MaxRetryCount} 次并发冲突");
            }
            catch (Exception ex)
            {
                // 记账失败不能影响聊天主流程
                LogHelper.logger.Error("Token 使用明细记录失败（已忽略）：" + ex.Message);
            }
        }
    }
}
