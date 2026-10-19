using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 用户月度 Token 额度服务
    /// </summary>
    public interface IAITokenQuotaService : IBaseService
    {
        /// <summary>
        /// 获取当前登录用户当月 Token 额度（无记录时按默认额度返回）
        /// </summary>
        Task<AIUserTokenQuotaDto> GetMyQuotaAsync();

        /// <summary>
        /// 聊天前置校验：当月额度已用完时抛用户友好业务异常，由统一异常处理返回给前端
        /// </summary>
        Task EnsureQuotaAvailableAsync();

        /// <summary>
        /// 每月初给所有用户颁发当月 Token 额度（已存在当月记录的用户跳过，Hangfire 定时任务调用）
        /// </summary>
        Task GrantMonthlyQuotaAsync();
    }
}
