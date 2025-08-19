using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class FileRp : Repository<TFile, Guid>, IFileRp
    {
        public FileRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
