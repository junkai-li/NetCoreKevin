namespace App.RepositorieRps.Repositories.v1
{
    /// <summary>
    /// AppDemo�ִ�����ӿ�
    /// </summary>

    public class AppDemoRp : Repository<TAppDemo, long>, IAppDemoRp
    {
        public AppDemoRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
