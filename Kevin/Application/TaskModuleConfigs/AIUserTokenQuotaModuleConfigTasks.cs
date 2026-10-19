using Hangfire;
using kevin.Domain.Interfaces.IServices.AI;
using Kevin.Hangfire.TieredServiceRegistration;

namespace kevin.Application.TaskServices
{
    /// <summary>
    /// 用户 Token 额度维护任务
    /// </summary>
    public class AIUserTokenQuotaModuleConfigTasks : IModuleConfigTasks
    {
        /// <summary>
        /// 配置任务
        /// </summary>
        public Task<bool> ConfigTasks(IRecurringJobManager recurringJobManager)
        {
            // 每月 1 号 00:00 给所有用户颁发当月 Token 额度（已有当月记录的用户跳过，幂等）
            recurringJobManager.AddOrUpdate<IAITokenQuotaService>(
                recurringJobId: "每月颁发用户Token额度",
                (s) => s.GrantMonthlyQuotaAsync(),
                "0 0 0 1 * ?", new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                }
            );
            return Task.FromResult(true);
        }
    }
}
