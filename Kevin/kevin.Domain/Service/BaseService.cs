using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Web.Base._;
using Web.Global.User;

namespace Web.Base;
public class BaseService : IBaseService
{
    public IServiceProvider _serviceProvider { get; set; } 
    public ICurrentUser CurrentUser { get; set; } 
    public IHttpContextAccessor HttpContextAccessor { get; set; }
    public BaseService()
    { 
    }
    public BaseService(IHttpContextAccessor _httpContextAccessor)
    {
        HttpContextAccessor = _httpContextAccessor; 
        _serviceProvider = _httpContextAccessor.HttpContext.RequestServices;
        CurrentUser = _httpContextAccessor.HttpContext.RequestServices.GetService<ICurrentUser>();
    }
}
