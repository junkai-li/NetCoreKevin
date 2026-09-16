using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace Kevin.SignalR
{
    public class MySignalRHub : Hub
    {
        public IServiceProvider serviceProvider { get; set; }

        public ICurrentUser _currentUser { get; set; }

        public string identityId { get; set; }


        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public MySignalRHub(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            if (serviceProvider != default)
            {
                var currentUser = serviceProvider.GetService<ICurrentUser>();
                if (currentUser != default)
                {
                    _currentUser = currentUser;
                }
            }
            if (httpContextAccessor != default)
            {
                _httpContextAccessor = httpContextAccessor;
            }
            identityId = GetIdentityId();
        }

        private string GetIdentityId()
        {
            if (_httpContextAccessor != default && _httpContextAccessor.HttpContext != default)
            {
                _httpContextAccessor = _httpContextAccessor;
                if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("IdentityId"))
                {
                    return _httpContextAccessor.HttpContext.Request.Headers["IdentityId"].FirstOrDefault() ?? "";
                }
                if (_httpContextAccessor.HttpContext.Request.Query["IdentityId"].FirstOrDefault() != default)
                {
                    return _httpContextAccessor.HttpContext.Request.Query["IdentityId"].FirstOrDefault() ?? "";
                }
                return _currentUser.UserId.ToString();
            }
            else
            {
                return _currentUser.UserId.ToString();
            }
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <returns></returns> 
        public override async Task OnConnectedAsync()
        {
            if (_currentUser == default)
            {
                throw new Exception($"用户不存在");
            }
            // 连接注册交给 SignalR 分组：组名就是推送目标用的 identityId，
            // 由 Redis 背板维护归属并在连接失效时自动回收，不再自己往缓存里维护整张映射表
            if (!string.IsNullOrEmpty(identityId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, identityId);
            }
            Console.WriteLine(identityId + "-链接MySignalRHub");
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // 分组归属由 SignalR 自己回收（连接结束即退出所有组），这里不需要再做清理
            Console.WriteLine(identityId + "断开链接MySignalRHub");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
