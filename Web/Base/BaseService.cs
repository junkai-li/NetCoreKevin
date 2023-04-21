
using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using Web.Base._;
using Web.Global;
using Web.Global.User;

namespace Web.Base;
public class BaseService : IBaseService
{
    public IServiceProvider _serviceProvider { get; set; }
    public dbContext db { get; set; } 
    public ICurrentUser CurrentUser { get; set; } 
    public IHttpContextAccessor HttpContextAccessor { get; set; }
    public BaseService()
    { 
    }
    public BaseService(IHttpContextAccessor _httpContextAccessor)
    {
        HttpContextAccessor = _httpContextAccessor; 
        _serviceProvider = _httpContextAccessor.HttpContext.RequestServices;
        db= _serviceProvider.GetService<dbContext>();
    }
}
