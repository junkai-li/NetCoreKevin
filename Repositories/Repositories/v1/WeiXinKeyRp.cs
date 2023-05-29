using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace App.RepositorieRps.Repositories.v1
{
    public class WeiXinKeyRp : Repository<TWeiXinKey, Guid>, IWeiXinKeyRp
    {
        public WeiXinKeyRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
