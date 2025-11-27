using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RepositorieRps.Repositories
{  
    public class DictionaryRp : Repository<TDictionary, long>, IDictionaryRp
    {
        public DictionaryRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
