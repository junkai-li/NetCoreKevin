using Hangfire;
using kevin.Domain.Interfaces.IServices.AI;
using Kevin.Hangfire.TieredServiceRegistration;

namespace kevin.Application.TaskServices
{
    /// <summary>
    /// AI 智能体记忆维护任务
    /// </summary>
    public class AIMemoryModuleConfigTasks : IModuleConfigTasks
    {
        /// <summary>
        /// 配置任务
        /// </summary>
        public Task<bool> ConfigTasks(IRecurringJobManager recurringJobManager)
        {
            // 每天凌晨 3 点清理已过期的短期记忆（软删除并同步删除 Qdrant 向量），操作幂等
            recurringJobManager.AddOrUpdate<IAIAgentMemoryService>(
                recurringJobId: "每天清理过期的智能体短期记忆",      // 唯一的 ID，用于后续修改或删除
                (s) => s.CleanupExpiredMemoriesAsync(),
                "0 0 3 * * ?", new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,        // 指定时区（默认UTC）
                }
            );
            return Task.FromResult(true);
        }
    }
}
