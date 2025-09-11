using kevin.Cache.Service;
using Kevin.SignalR.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Web.Global.User;

namespace Kevin.SignalR
{
    public class MySignalRHub : Hub
    {
        public ICurrentUser _currentUser { get; set; }

        public ICacheService _cacheService { get; set; }

        private readonly SignalrRdisSetting _config;

        public MySignalRHub(ICurrentUser currentUser, IOptionsMonitor<SignalrRdisSetting> config, ICacheService cacheService)
        {
            _currentUser = currentUser;
            _cacheService = cacheService;
            _config = config.CurrentValue;
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
            if (_currentUser.UserId != default && _currentUser.TenantId != default)
            {
                var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                if (tenantData != default)
                {
                    if (tenantData.UserConnections.FirstOrDefault(t => t.UserId == _currentUser.UserId.ToString()) != default)
                    {
                        tenantData.UserConnections.RemoveAll(t => t.UserId == _currentUser.UserId.ToString());
                    }
                    ;
                    tenantData.UserConnections?.Add(new UserConnectionDto
                    {
                        UserId = _currentUser.UserId.ToString(),
                        ConnectionId = Context.ConnectionId
                    });
                    _cacheService.SetObject(_config.cacheMySignalRKeyName, data);
                }
                else
                {
                    data.Items.Add(new SignalRRedisItemDto
                    {
                        TenantId = _currentUser.TenantId,
                        UserConnections = new List<UserConnectionDto> {
                                                          new UserConnectionDto{ ConnectionId=Context.ConnectionId, UserId=_currentUser.UserId.ToString() }
                                                            }
                    });
                }
            }
            // Console.WriteLine(_currentUser.UserId + "链接");
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (_currentUser.UserId != default && _currentUser.TenantId != default)
                {
                    var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                    var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                    if (tenantData != default)
                    {
                        if (tenantData.UserConnections.FirstOrDefault(t => t.UserId == _currentUser.UserId.ToString()) != default)
                        {
                            tenantData.UserConnections.RemoveAll(t => t.UserId == _currentUser.UserId.ToString());
                        }
                    }
                } 
                // Console.WriteLine(Context.ConnectionId + "断开");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public new void Dispose()
        {
            _cacheService.Remove(_config.cacheMySignalRKeyName);
            base.Dispose(); ;
        }
        /// <summary>
        /// 发送公告消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendPublicMsg(string msg)
        {
            return this.Clients.All.SendAsync("AllMsg", msg);
        }
        /// <summary>
        /// 私发信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendUserMsg(string connId, string msg)
        {
            string newmsg = $"{connId}{DateTime.Now}:{msg}";
            return this.Clients.Client(connId).SendAsync("ReceptionUserMsg", newmsg);
        }
    }
}
