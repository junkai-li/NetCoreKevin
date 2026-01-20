using kevin.Cache.Service;
using Kevin.Common.Extension;
using Kevin.SignalR.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.User;

namespace Kevin.SignalR.Service
{
    public class SignalRMsgService : ISignalRMsgService
    {
        private readonly IHubContext<MySignalRHub> _messageHub;

        private readonly SignalrRdisSetting _config;
        public ICurrentUser _currentUser { get; set; }

        private readonly ICacheService _cacheService;
        public SignalRMsgService(IHubContext<MySignalRHub> messageHub, IOptionsMonitor<SignalrRdisSetting> config, ICacheService cacheService, ICurrentUser currentUser)
        {
            _messageHub = messageHub;
            _config = config.CurrentValue;
            _cacheService = cacheService;
            _currentUser = currentUser;
        }

        public List<string> GetTenantConnIds(int TenantId)
        {
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            if (data != default && data.Items != default)
            {
                return data.Items.FirstOrDefault(t => t.TenantId == TenantId)?.Connections?.Select(t => t.ConnectionId).ToList() ?? new List<string>();
            }
            return new List<string>();
        }

        public List<string> GetTenantIdentityIds(int TenantId)
        {
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            if (data != default && data.Items != default)
            {
                return data.Items.FirstOrDefault(t => t.TenantId == TenantId)?.Connections?.Select(t => t.IdentityId).ToList() ?? new List<string>();
            }
            return new List<string>();
        }

        public string GetIdentityConnId(string identityId)
        {
            string id = "";
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            if (data != default)
            {
                foreach (var item in data.Items)
                {
                    if (item.Connections.Where(t => t.IdentityId == identityId).FirstOrDefault() != default)
                    {
                        id = item.Connections.Where(t => t.IdentityId == identityId).FirstOrDefault()?.ConnectionId ?? "";
                        return id;
                    }
                }
            }
            return id;
        }

        public Task SendPublicMsg(string method, string msg)
        {
            return _messageHub.Clients.All.SendAsync(method, msg);
        }

        public Task SendConnIdMsg(string method, string connId, string msg)
        {
            return _messageHub.Clients.Client(connId).SendAsync(method, msg);
        }
        public Task SendConnIdsMsg(string method, List<string> connIds, string msg)
        {
            return _messageHub.Clients.Clients(connIds).SendAsync(method, msg);
        }

        public Task SendIdentityIdMsg(string method, string identityId, string msg)
        {
            return _messageHub.Clients.Client(GetIdentityIdConnIds(new List<string>() { identityId }).FirstOrDefault() ?? identityId).SendAsync(method, msg);
        }
        public Task SendIdentityIdsMsg(string method, List<string> identityIds, string msg)
        {
            return _messageHub.Clients.Clients(GetIdentityIdConnIds(identityIds)).SendAsync(method, msg);
        }

        private List<string> GetIdentityIdConnIds(List<string> identityId)
        {
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            var ids = new List<string>();
            if (data != default)
            {
                foreach (var item in data.Items)
                {
                    ids.AddRange(item.Connections.Where(t => identityId.Contains(t.IdentityId)).Select(t => t.ConnectionId).ToList());
                }
            }
            return ids;
        }
    }
}
