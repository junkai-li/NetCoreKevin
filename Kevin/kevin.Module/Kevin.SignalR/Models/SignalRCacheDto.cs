using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SignalR.Models
{
    public class SignalRCacheDto
    {
        /// <summary>
        /// 用户组列表
        /// </summary>
        public List<SignalRRedisItemDto> Items { get; set; } = new List<SignalRRedisItemDto>();
    }
    public class SignalRRedisItemDto
    {

        /// <summary>
        ///租户id
        /// </summary>
        public int TenantId { get; set; } = 1000;

        /// <summary>
        /// 用户链接ids
        /// </summary>
        public List<UserConnectionDto> UserConnections { get; set; } = new List<UserConnectionDto>();
    }
    public class UserConnectionDto
    {
        public string UserId { get; set; } = "";
        public string ConnectionId { get; set; } = "";

    }

}
