using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Entities;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.RepositorieRps.Repositories.v1
{
    public class TenantRp : Repository<TTenant, Guid>, ITenantRp
    {
        public TenantRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
