
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using Web.Base._;
using Web.Global;
using Web.Global.User;

namespace Web.Base;
public class BaseService : IBaseService {
    private IServiceProvider Provider => GlobalServices.ServiceProvider;

    public dbContext db { get => Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<dbContext>();  }
    public ICurrentUser CurrentUser { get => Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<ICurrentUser>();}
     
     
}
