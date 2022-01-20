
using Ax.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using Web.Base._;
using Web.Global;
using Web.Global.User;

namespace Web.Base;
public abstract class BaseRepository<T, IdType> : Repository<T, IdType> where T : class
{
    public BaseRepository(dbContext context) : base(context)
    {
    }
    private IServiceProvider Provider => GlobalServices.ServiceProvider;

    public ICurrentUser CurrentUser { get => Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<ICurrentUser>(); }
}
