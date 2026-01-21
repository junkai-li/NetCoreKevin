using kevin.Cache.Service;
using Kevin.SignalR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Web.Global.User;

namespace Kevin.SignalR
{
    public class MySignalRHub : Hub
    {
        public IServiceProvider serviceProvider { get; set; }

        public ICurrentUser _currentUser { get; set; }

        public string identityId { get; set; }

        public ICacheService _cacheService { get; set; }

        public IHttpContextAccessor _httpContextAccessor { get; set; }

        private readonly SignalrRdisSetting _config;

        public MySignalRHub(IOptionsMonitor<SignalrRdisSetting> config, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            if (serviceProvider != default)
            {
                _currentUser = serviceProvider.GetService<ICurrentUser>();
                _cacheService = serviceProvider.GetService<ICacheService>();
            }
            _config = config.CurrentValue;
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
            if (identityId != default && _currentUser.TenantId != default)
            {
                var data = new SignalRCacheDto();
                var json = _cacheService.GetString(_config.cacheMySignalRKeyName);
                if (!string.IsNullOrEmpty(json))
                {
                    data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                    if (data != default)
                    {
                        var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                        if (tenantData != default)
                        {
                            if (tenantData.Connections.FirstOrDefault(t => t.IdentityId == identityId) != default)
                            {
                                tenantData.Connections.RemoveAll(t => t.IdentityId == identityId);
                            }
                            tenantData.Connections?.Add(new IdentityConnectionDto
                            {
                                IdentityId = identityId,
                                ConnectionId = Context.ConnectionId
                            });
                        }
                        else
                        {
                            data.Items.Add(new SignalRRedisItemDto
                            {
                                TenantId = _currentUser.TenantId,
                                Connections = new List<IdentityConnectionDto> {
                                                          new IdentityConnectionDto{ ConnectionId=Context.ConnectionId, IdentityId=identityId}
                                                            }
                            });
                        }
                    }

                }
                else
                {
                    data.Items.Add(new SignalRRedisItemDto
                    {
                        TenantId = _currentUser.TenantId,
                        Connections = new List<IdentityConnectionDto> {
                                                          new IdentityConnectionDto{ ConnectionId=Context.ConnectionId, IdentityId=identityId}
                                                            }
                    });
                }
                _cacheService.SetObject(_config.cacheMySignalRKeyName, data);
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
            try
            {
                if (identityId != default && _currentUser.TenantId != default)
                {
                    var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                    if (data != default)
                    {
                        var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                        if (tenantData != default)
                        {
                            if (tenantData.Connections.FirstOrDefault(t => t.IdentityId == identityId) != default)
                            {
                                tenantData.Connections.RemoveAll(t => t.IdentityId == identityId);
                            }
                        }
                        _cacheService.SetObject(_config.cacheMySignalRKeyName, data);
                    }
                  
                }
                Console.WriteLine(identityId + "断开链接MySignalRHub");
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
    }
}
