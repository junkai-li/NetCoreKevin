using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{  
    public class DictionaryRp : Repository<TDictionary, long>, IDictionaryRp
    {
        public DictionaryRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
