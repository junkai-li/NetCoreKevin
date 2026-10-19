using kevin.AI.AgentFramework.Dto;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.log4Net;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 用户月度 Token 额度服务
    /// <para>
    /// 额度按“租户 + 用户 + 月份”唯一：每月初由定时任务给所有用户颁发默认额度，
    /// 聊天完成后由聊天历史领域事件累加已用额度并写入使用明细。
    /// </para>
    /// </summary>
    public class AITokenQuotaService : BaseService, IAITokenQuotaService
    {
        /// <summary>
        /// 每月默认颁发额度：默认 2 亿 Token，可在 appsettings 的 AITokenQuotaSetting:MonthlyQuota 调整
        /// </summary>
        public static long DefaultMonthlyQuota => AITokenQuotaSetting.Current.MonthlyQuota;

        /// <summary>
        /// 颁发任务每批处理的用户数，避免一次性加载全部用户
        /// </summary>
        private const int GrantBatchSize = 500;

        public IAIUserTokenQuotaRp AIUserTokenQuotaRp { get; set; }

        public IUserRp UserRp { get; set; }

        public AITokenQuotaService(IHttpContextAccessor httpContextAccessor, IAIUserTokenQuotaRp aiUserTokenQuotaRp, IUserRp userRp) : base(httpContextAccessor)
        {
            AIUserTokenQuotaRp = aiUserTokenQuotaRp;
            UserRp = userRp;
        }

        /// <summary>
        /// 按时间取额度月份（yyyyMM）
        /// </summary>
        public static int GetQuotaMonth(DateTime time)
        {
            return time.Year * 100 + time.Month;
        }

        public async Task<AIUserTokenQuotaDto> GetMyQuotaAsync()
        {
            var userId = CurrentUser.UserId;
            if (userId <= 0)
            {
                throw new UserFriendlyException("无法获取当前用户，请先登录");
            }
            var tenantId = CurrentUser.TenantId;
            var quotaMonth = GetQuotaMonth(DateTime.Now);
            var data = await AIUserTokenQuotaRp.Query(isTenant: false, isDataPer: false)
                .Where(t => t.IsDelete == false && t.UserId == userId && t.TenantId == tenantId && t.QuotaMonth == quotaMonth)
                .FirstOrDefaultAsync();

            // 当月记录还没颁发（月初任务未执行/新用户当月首登）时按默认额度展示，不产生写库副作用
            var totalQuota = data?.TotalQuota ?? DefaultMonthlyQuota;
            var usedQuota = data?.UsedQuota ?? 0;
            return new AIUserTokenQuotaDto
            {
                UserId = userId,
                QuotaMonth = quotaMonth,
                TotalQuota = totalQuota,
                UsedQuota = usedQuota,
                RemainingQuota = Math.Max(0, totalQuota - usedQuota)
            };
        }

        public async Task EnsureQuotaAvailableAsync()
        {
            var userId = CurrentUser.UserId;
            // 无法识别当前用户（系统内部调用）时不拦截，避免影响非用户态流程
            if (userId <= 0)
            {
                return;
            }
            var tenantId = CurrentUser.TenantId;
            var quotaMonth = GetQuotaMonth(DateTime.Now);
            var data = await AIUserTokenQuotaRp.Query(isTenant: false, isDataPer: false)
                .Where(t => t.IsDelete == false && t.UserId == userId && t.TenantId == tenantId && t.QuotaMonth == quotaMonth)
                .FirstOrDefaultAsync();
            // 当月额度还没颁发（月初任务未执行/新用户首登）时按默认额度放行，由事件处理器兜底建账
            if (data == default)
            {
                return;
            }
            if (data.UsedQuota >= data.TotalQuota)
            {
                throw new UserFriendlyException($"本月 AI Token 额度已用完（已用 {FormatToken(data.UsedQuota)} / 总额度 {FormatToken(data.TotalQuota)}），额度将于每月 1 日自动重置，请下月再试。");
            }
        }

        /// <summary>
        /// 提示文案中的额度展示：亿/万，避免直接丢出一长串数字
        /// </summary>
        private static string FormatToken(long value)
        {
            if (value >= 100_000_000)
            {
                return $"{value / 100_000_000d:0.##} 亿";
            }
            if (value >= 10_000)
            {
                return $"{value / 10_000d:0.##} 万";
            }
            return value.ToString();
        }

        public async Task GrantMonthlyQuotaAsync()
        {
            var quotaMonth = GetQuotaMonth(DateTime.Now);
            var pageIndex = 0;
            var grantedCount = 0;
            while (true)
            {
                // 定时任务无登录用户，必须关闭租户与数据权限过滤，否则只能查到 0 个用户
                var users = await UserRp.Query(isTenant: false, isDataPer: false)
                    .Where(t => t.IsDelete == false)
                    .OrderBy(t => t.Id)
                    .Skip(pageIndex * GrantBatchSize)
                    .Take(GrantBatchSize)
                    .Select(t => new { t.Id, t.TenantId })
                    .ToListAsync();
                if (users.Count == 0)
                {
                    break;
                }

                var userIds = users.Select(t => t.Id).ToList();
                var existsUserIds = await AIUserTokenQuotaRp.Query(isTenant: false, isDataPer: false)
                    .Where(t => t.QuotaMonth == quotaMonth && userIds.Contains(t.UserId))
                    .Select(t => t.UserId)
                    .ToListAsync();

                var addList = users.Where(t => !existsUserIds.Contains(t.Id)).Select(t => new TAIUserTokenQuota
                {
                    Id = SnowflakeIdService.GetNextId(),
                    UserId = t.Id,
                    QuotaMonth = quotaMonth,
                    TotalQuota = DefaultMonthlyQuota,
                    UsedQuota = 0,
                    IsDelete = false,
                    CreateTime = DateTime.Now,
                    TenantId = t.TenantId
                }).ToList();
                if (addList.Count > 0)
                {
                    AIUserTokenQuotaRp.AddRange(addList);
                    await AIUserTokenQuotaRp.SaveChangesAsync();
                    grantedCount += addList.Count;
                }

                if (users.Count < GrantBatchSize)
                {
                    break;
                }
                pageIndex++;
            }
            LogHelper.logger.Info($"用户月度Token额度颁发完成：月份 {quotaMonth}，新增 {grantedCount} 条，单用户额度 {DefaultMonthlyQuota}");
        }
    }
}
