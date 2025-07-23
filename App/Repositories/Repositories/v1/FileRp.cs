using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace App.RepositorieRps.Repositories.v1
{
    public class FileRp : Repository<TFile, Guid>, IFileRp
    {
        public FileRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
