using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class OSLogRp : Repository<TOSLog, long>, IOSLogRp
    {
        public OSLogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}
