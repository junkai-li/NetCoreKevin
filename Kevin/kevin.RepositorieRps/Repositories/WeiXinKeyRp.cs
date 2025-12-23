using kevin.Domain.Entities;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class WeiXinKeyRp : Repository<TWeiXinKey, long>, IWeiXinKeyRp
    {
        public WeiXinKeyRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
