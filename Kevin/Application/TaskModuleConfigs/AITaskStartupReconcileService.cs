using kevin.AI.AgentFramework.Interfaces.Tasks;
using Medallion.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kevin.log4Net;

namespace kevin.Application.Services.AI.Tools
{
    /// <summary>
    /// AI 自动任务启动对账服务：应用启动后以 TAITaskConfig 为权威源，把任务状态同步到 Hangfire。
    /// <para>
    /// 双存储兼容（设计约束）：
    /// - 内存存储（开发/单机）：重启后 RecurringJob/Scheduled 作业全部丢失，由本服务从落库配置重建未暂停任务——顺带修复"重启丢任务"的存量隐患；
    /// - Redis 持久化存储（正式环境）：作业重启后仍在，AddOrUpdate 幂等重放等于 no-op；一次性任务先扫 Scheduled 队列去重，防止重启后双排；
    /// - 暂停任务在任何存储下都会被 RemoveIfExists/删除作业，不会"复活"。
    /// </para>
    /// 多实例部署用分布式锁互斥；对账失败自动重试（最多 5 次），不阻塞应用启动。
    /// </summary>
    public class AITaskStartupReconcileService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDistributedLockProvider _distLock;

        public AITaskStartupReconcileService(IServiceScopeFactory scopeFactory, IDistributedLockProvider distLock)
        {
            _scopeFactory = scopeFactory;
            _distLock = distLock;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 延迟启动：等 Hangfire Server 与数据库迁移就绪
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            const int maxRetry = 5;
            for (var attempt = 1; attempt <= maxRetry; attempt++)
            {
                if (stoppingToken.IsCancellationRequested) return;
                try
                {
                    // 多实例互斥：防止并发对账导致一次性任务双排（周期性 AddOrUpdate 本身幂等）；与项目惯例一致用同步取锁
                    var lockHandle = _distLock.TryAcquireLock("kevin.AI.AITaskStartupReconcile", TimeSpan.FromSeconds(30));
                    if (lockHandle == null)
                    {
                        LogHelper.logger.Info("自动任务启动对账：其他实例正在执行，本实例跳过");
                        return;
                    }
                    using (lockHandle)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var taskService = scope.ServiceProvider.GetRequiredService<IKevinAITaskService>();
                        await taskService.ReconcileTasksFromConfigAsync();
                    }
                    return;
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error($"自动任务启动对账失败（第 {attempt}/{maxRetry} 次）：{ex.Message}", ex);
                    if (attempt < maxRetry)
                    {
                        try { await Task.Delay(TimeSpan.FromSeconds(10 * attempt), stoppingToken); }
                        catch (TaskCanceledException) { return; }
                    }
                }
            }
        }
    }
}
