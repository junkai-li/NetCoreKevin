using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class LogRp : Repository<TLog, long>, ILogRp
    {
        public LogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
