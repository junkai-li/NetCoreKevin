namespace App.RepositorieRps.Repositories.v1
{
    /// <summary>
    /// AppCodeTest�ִ�����ӿ�
    /// </summary>

    public class AppCodeTestRp : Repository<TAppCodeTest, long>, IAppCodeTestRp
    {
        public AppCodeTestRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
