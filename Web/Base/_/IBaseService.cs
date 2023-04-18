
using Repository.Database;
using Web.Global.User;

namespace Web.Base._
{
    public interface IBaseService
    {
        dbContext db { get;}

        ICurrentUser CurrentUser { get;}
    }
}
