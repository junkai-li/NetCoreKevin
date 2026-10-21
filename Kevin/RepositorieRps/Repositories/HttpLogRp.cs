using Kevin.Common.App;
using Kevin.log4Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;

namespace kevin.RepositorieRps.Repositories
{
    public class HttpLogRp : Repository<THttpLog, long>, IHttpLogRp
    {

        public HttpLogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        /// <summary>
        /// 上一次输出“取不到 HttpContext”告警的时间，用于限流，避免刷爆日志
        /// </summary>
        private static DateTime _lastMissingContextWarnTime = DateTime.MinValue;

        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="operateRemark"></param>
        /// <returns></returns>
        public async Task<bool> Add(string operateType, string operateRemark)
        {
            await using var db = new KevinDbContext();

            //HttpContext 在部分链路（后台任务、连接已结束、非请求作用域）下可能取不到。
            //IP/Url/HttpMethod 这些请求相关字段留空即可，绝不能让日志写入把业务请求打挂。
            //注意：Current() 取不到时返回 null，而 GetIpAddress/GetRequestBody 等都是扩展方法，
            //接收者为 null 时内部已做判空，只有 Current().Request.Method 这一处必须自己兜住。
            var httpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.Current();
            if (httpContext == null)
            {
                WarnMissingHttpContextOnce(httpContextAccessor == null, CurrentUser?.UserId ?? 0);
            }

            var log = new THttpLog();
            log.Id = SnowflakeIdService.GetNextId();
            log.CreateTime = DateTime.Now;
            log.IsDelete = false;
            log.CreateUserId = log.Id;
            log.UserName = "未知";
            log.TenantId = CurrentUser.TenantId;
            if (CurrentUser != default)
            {
                log.CreateUserId = CurrentUser.UserId.ToTryInt64() > 0 ? CurrentUser.UserId.ToTryInt64() : log.Id.ToTryInt64();
                log.UserName = CurrentUser.UserName ?? "未知";
            }
            log.OperateRemark = operateRemark;
            log.OperateType = operateType;
            log.IP = httpContextAccessor.GetIpAddress();
            log.HttpBody = httpContextAccessor.GetRequestBody();
            log.HttpAction = httpContextAccessor.GetUrlAction();
            log.HttpUrl = httpContextAccessor.GetUrl();
            log.Device = httpContextAccessor.GetDevice();
            log.HttpMethod = httpContext?.Request?.Method ?? "";
            db.Set<THttpLog>().Add(log);
            await db.SaveChangesAsync(true);
            return true;
        }

        /// <summary>
        /// 取不到 HttpContext 时限流输出一次告警，便于定位是 accessor 未注入还是上下文已结束
        /// </summary>
        private static void WarnMissingHttpContextOnce(bool accessorIsNull, long currentUserId)
        {
            var now = DateTime.Now;
            if ((now - _lastMissingContextWarnTime).TotalMinutes < 1)
            {
                return;
            }
            _lastMissingContextWarnTime = now;
            LogHelper<HttpLogRp>.logger.Warn($"写入请求日志时取不到 HttpContext（IHttpContextAccessor 是否为 null：{accessorIsNull}，CurrentUser.UserId：{currentUserId}），本次 IP/Url/HttpMethod 等字段按空值落库");
        }
    }
}
