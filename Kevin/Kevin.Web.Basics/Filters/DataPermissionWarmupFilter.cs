using Kevin.log4Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace Kevin.Web.Filters
{
    /// <summary>
    /// 数据权限预热过滤器。
    /// 仓储在构造查询表达式时是同步的（Expression 内无法 await），
    /// 这里先把「当前用户可查看的数据范围」异步算好放进请求级缓存，
    /// 后续 Query/FirstOrDefault/Any 直接读缓存，不再用 .Result 阻塞线程池线程。
    /// </summary>
    public class DataPermissionWarmupFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var currentUser = context.HttpContext.RequestServices.GetService<ICurrentUser>();
                //UserId 为 0 表示未登录/后台线程，此时仓储的数据权限分支本身也不会执行
                if (currentUser != null && currentUser.UserId > 0)
                {
                    await currentUser.GetModuleDataPermissionsUserIds();
                }
            }
            catch (Exception ex)
            {
                LogHelper<DataPermissionWarmupFilter>.logger.Error($"数据权限预热失败: {ex.Message}", ex);
            }

            await next();
        }
    }
}
