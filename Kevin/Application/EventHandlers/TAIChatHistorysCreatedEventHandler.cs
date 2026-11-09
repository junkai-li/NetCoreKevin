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
    /// <para>
    /// 明细靠 AIChatHistorysId 唯一索引幂等；额度累加走数据库端原子自增，不再“读出实体→改属性→SaveChanges”：
    /// 额度行带 [ConcurrencyCheck] RowVersion，同一用户并发多轮记账时读改存会互相顶掉（UPDATE 命中 0 行），
    /// 重试只是让并发者排队，并发数超过重试次数就会持续报“并发冲突”。
    /// </para>
    /// </summary>
    public class TAIChatHistorysCreatedEventHandler : INotificationHandler<TAIChatHistorysCreatedEvent>
    {
        /// <summary>
        /// 写库失败时的最大尝试次数
        /// </summary>
        private const int MaxRetryCount = 3;

        /// <summary>
        /// 重试退避基数（毫秒）：第 n 次重试等待 n * 该值，给并发事务留出提交窗口
        /// </summary>
        private const int RetryDelayMilliseconds = 50;

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
                        await RecordAsync(chat, userId, quotaMonth, inputTokenCount, outputTokenCount, totalTokenCount);
                        return;
                    }
                    catch (DbUpdateException ex)
                    {
                        // 明细与额度在同一本地事务里，失败已整体回滚；带上真实异常信息，便于区分锁冲突与结构/字段错误
                        LogHelper.logger.Info($"Token 记账写库失败，第 {retry + 1} 次重试：聊天记录 {chat.Id}，{ex.GetBaseException().Message}");
                        if (retry < MaxRetryCount - 1)
                        {
                            await Task.Delay(RetryDelayMilliseconds * (retry + 1), CancellationToken.None);
                        }
                    }
                }
                LogHelper.logger.Error($"Token 使用明细记录失败：聊天记录 {chat.Id} 连续 {MaxRetryCount} 次写库失败");
            }
            catch (Exception ex)
            {
                // 记账失败不能影响聊天主流程
                LogHelper.logger.Error("Token 使用明细记录失败（已忽略）：" + ex.Message);
            }
        }

        /// <summary>
        /// 明细 + 额度一次落地：明细由 AIChatHistorysId 唯一索引去重，额度直接在库里自增，两者在同一本地事务内。
        /// <para>明细已存在（事件重复发布/并发重复触发）时整笔跳过，避免重复累加额度。</para>
        /// </summary>
        private async Task RecordAsync(TAIChatHistorys chat, long userId, int quotaMonth, long inputTokenCount, long outputTokenCount, long totalTokenCount)
        {
            using var db = new KevinDbContext();
            await using var transaction = await db.Database.BeginTransactionAsync(CancellationToken.None);
            try
            {
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
                try
                {
                    await db.SaveChangesAsync(CancellationToken.None);
                }
                catch (DbUpdateException ex) when (IsDuplicateKey(ex))
                {
                    // 同一条聊天记录只记一次明细（事件重复发布保护）
                    await transaction.RollbackAsync(CancellationToken.None);
                    return;
                }

                await AccrueUsedQuotaAsync(db, chat, userId, quotaMonth, totalTokenCount);
                await transaction.CommitAsync(CancellationToken.None);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        /// <summary>
        /// 累加当月已用额度：先按条件在库里直接自增（used_quota = used_quota + n），不做行版本校验；
        /// 影响 0 行说明当月额度行还没建，兜底建一行；并发撞唯一键时改为自增，不会再互相顶掉。
        /// </summary>
        private async Task AccrueUsedQuotaAsync(KevinDbContext db, TAIChatHistorys chat, long userId, int quotaMonth, long totalTokenCount)
        {
            if (await IncreaseUsedQuotaAsync(db, chat, userId, quotaMonth, totalTokenCount) > 0)
            {
                return;
            }

            // 月初任务尚未执行（或新用户当月首次对话）时兜底创建当月额度，避免用量丢失
            var quota = new TAIUserTokenQuota
            {
                Id = _snowflakeIdService.GetNextId(),
                UserId = userId,
                QuotaMonth = quotaMonth,
                TotalQuota = AITokenQuotaService.DefaultMonthlyQuota,
                UsedQuota = totalTokenCount,
                IsDelete = false,
                CreateTime = DateTime.Now,
                TenantId = chat.TenantId
            };
            db.Set<TAIUserTokenQuota>().Add(quota);
            try
            {
                await db.SaveChangesAsync(CancellationToken.None);
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex))
            {
                // 并发方刚建好当月额度行：把这条待插入实体摘出跟踪器，改为在库里自增
                db.Entry(quota).State = EntityState.Detached;
                await IncreaseUsedQuotaAsync(db, chat, userId, quotaMonth, totalTokenCount);
            }
        }

        /// <summary>
        /// 库内原子自增当月已用额度，返回受影响行数（0 表示当月额度行不存在）
        /// </summary>
        private static Task<int> IncreaseUsedQuotaAsync(KevinDbContext db, TAIChatHistorys chat, long userId, int quotaMonth, long totalTokenCount)
        {
            return db.Set<TAIUserTokenQuota>()
                .Where(t => t.IsDelete == false && t.UserId == userId && t.TenantId == chat.TenantId && t.QuotaMonth == quotaMonth)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.UsedQuota, t => t.UsedQuota + totalTokenCount), CancellationToken.None);
        }

        /// <summary>
        /// 判断是否为唯一索引冲突（MySQL 1062）。本层不直接引用数据库驱动类型，先反射取 Number，再按消息兜底。
        /// </summary>
        private static bool IsDuplicateKey(Exception ex)
        {
            var inner = ex.GetBaseException();
            if (inner.GetType().GetProperty("Number")?.GetValue(inner) is int code)
            {
                return code == 1062;
            }
            return inner.Message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase)
                || inner.Message.Contains("Duplicate key", StringComparison.OrdinalIgnoreCase);
        }
    }
}
