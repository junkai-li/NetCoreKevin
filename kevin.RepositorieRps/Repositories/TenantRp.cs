using kevin.Domain.Entities;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories 
{
    public class TenantRp : Repository<TTenant, Guid>, ITenantRp
    {
        public TenantRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
