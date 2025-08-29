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

namespace Kevin.SignalR.Service
{
    public class SignalRMsgService : ISignalRMsgService
    {
        private readonly IHubContext<MySignalRHub> _messageHub;

        private readonly SignalrRdisSetting _config;

        private readonly ICacheService _cacheService;
        public SignalRMsgService(IHubContext<MySignalRHub> messageHub, IOptionsMonitor<SignalrRdisSetting> config, ICacheService cacheService)
        {
            _messageHub = messageHub;
            _config = config.CurrentValue;
            _cacheService = cacheService;
        }

        public List<string> GetTenantConnIds(int TenantId)
        {
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            if (data != default && data.Items != default)
            {
                return data.Items.FirstOrDefault(t => t.TenantId == TenantId)?.UserConnections?.Select(t => t.ConnectionId).ToList() ?? new List<string>();
            }
            return new List<string>();
        }

        public string GetUserConnId(string userId)
        {
            string id = "";
            var data = _cacheService.GetObject<SignalRCacheDto>(_config.cacheMySignalRKeyName);
            if (data != default)
            {
                foreach (var item in data.Items)
                {
                    if (item.UserConnections.Where(t => t.UserId == userId).FirstOrDefault() != default)
                    {
                        id = item.UserConnections.Where(t => t.UserId == userId).FirstOrDefault()?.ConnectionId ?? "";
                        return id;
                    }
                }
            }
            return id;
        }

        public Task SendPublicMsg(string msg)
        {
            return _messageHub.Clients.All.SendAsync("AllMsg", msg);
        }

        public Task SendUserMsg(string connId, string msg)
        {
            return _messageHub.Clients.Client(connId).SendAsync("ReceptionUserMsg", msg);
        }
        public Task SendUsersMsg(List<string> connIds, string msg)
        {
            return _messageHub.Clients.Clients(connIds).SendAsync("ReceptionUserMsg", msg);
        }
    }
}
