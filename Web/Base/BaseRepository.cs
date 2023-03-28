
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
    protected IServiceProvider ServiceProvider { get; }
    public BaseRepository(dbContext context,IServiceProvider serviceProvider) : base(context)
    {
        ServiceProvider = serviceProvider;
    } 
    public ICurrentUser CurrentUser { get => ServiceProvider.GetService<ICurrentUser>(); }
}
