namespace App.RepositorieRps.Repositories.v1
{  
    public class AppDemoRp : Repository<TAppDemo, long>, IAppDemoRp
    {
        public AppDemoRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
