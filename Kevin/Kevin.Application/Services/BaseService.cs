using kevin.Domain.Share;
using Kevin.SnowflakeId.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace kevin.Application;
public class BaseService : IBaseService
{
    public IServiceProvider? _serviceProvider { get; set; }
    public required ICurrentUser CurrentUser { get; set; }
    public required IHttpContextAccessor HttpContextAccessor { get; set; }

    public required ISnowflakeIdService SnowflakeIdService { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public BaseService()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_httpContextAccessor"></param>
    public BaseService(IHttpContextAccessor _httpContextAccessor)
    {
        HttpContextAccessor = _httpContextAccessor;
        if (_httpContextAccessor.HttpContext != default)
        {
            _serviceProvider = _httpContextAccessor.HttpContext.RequestServices;
            CurrentUser = _serviceProvider.GetService<ICurrentUser>() ?? new CurrentUser(_httpContextAccessor);
            SnowflakeIdService = _serviceProvider.GetService<ISnowflakeIdService>() ?? new SnowflakeIdService();
        }
    }
}
