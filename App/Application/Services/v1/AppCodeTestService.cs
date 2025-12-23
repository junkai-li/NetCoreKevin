namespace App.Application.Services.v1
{
    /// <summary>
    /// AppCodeTest服务接口
    /// </summary>
    public class AppCodeTestService : BaseService, IAppCodeTestService
    {
        public IAppCodeTestRp AppCodeTestRp { get; set; }
        public AppCodeTestService(IHttpContextAccessor _httpContextAccessor, IAppCodeTestRp _AppCodeTestRp) : base(_httpContextAccessor)
        {
            this.AppCodeTestRp = _AppCodeTestRp;
        }
    }
}
