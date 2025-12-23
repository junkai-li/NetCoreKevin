namespace App.Application.Services.v1
{
    /// <summary>
    /// AppDemo服务接口
    /// </summary>
    public class AppDemoService : BaseService, IAppDemoService
    {
        public IAppDemoRp AppDemoRp { get; set; }
        public AppDemoService(IHttpContextAccessor _httpContextAccessor, IAppDemoRp _AppDemoRp) : base(_httpContextAccessor)
        {
            this.AppDemoRp = _AppDemoRp;
        }
    }
}
