using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace App.RepositorieRps.Repositories.v1
{
    public class LogRp : Repository<TLog, Guid>, ILogRp
    {
        public LogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
