
using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;
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
    private IServiceProvider Provider { get; set; } 
    public dbContext db { get; set; } 
    public ICurrentUser CurrentUser { get; set; }

    public BaseService(dbContext context, IServiceProvider serviceProvider)
    {
        db = context;
        serviceProvider = Provider;
    }
}
