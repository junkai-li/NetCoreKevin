using kevin.Domain.Share.Interfaces;
using Web.Global.User;

namespace kevin.Domain.Share
{
    public partial interface IBaseService: IService
    {  
        ICurrentUser CurrentUser { get;}
    }
}
