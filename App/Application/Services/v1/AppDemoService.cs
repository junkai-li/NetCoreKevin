namespace App.Application.Services.v1
{
    public class AppDemoService : BaseService, IAppDemoService
    {
        public IAppDemoRp appDemoRp { get; set; }
        public AppDemoService(IHttpContextAccessor _httpContextAccessor, IAppDemoRp _appDemoRp) : base(_httpContextAccessor)
        {
            this.appDemoRp = _appDemoRp;
        }
    }
}
