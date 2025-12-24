using kevin.Domain.Entities;
using Kevin.EntityFrameworkCore._.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RepositorieRps.Repositories
{ 
    public class UserBindPositionRp : Repository<TUserBindPosition, long>, IUserBindPositionRp
    {
        public UserBindPositionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
