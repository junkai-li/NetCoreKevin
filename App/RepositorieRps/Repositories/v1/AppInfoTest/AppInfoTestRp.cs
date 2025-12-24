using App.Domain.Entities.AppInfoTest;
using  App.Domain.Interfaces.Repositorie.v1.AppInfoTest;
namespace App.RepositorieRps.Repositories.v1.AppInfoTest
{
    /// <summary>
    /// AppInfoTest�ִ�����ӿ�
    /// </summary>

    public class AppInfoTestRp : Repository<TAppInfoTest, long>, IAppInfoTestRp
    {
        public AppInfoTestRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
