using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 专门用于存储AI聊天记录的表
    /// </summary>
    [Table("TAIChatMessageStore")]
    [Description("专门用于存储AI聊天记录的表")]
    [Index(nameof(Key))]
    [Index(nameof(ThreadId))]
    [Index(nameof(Role))]
    [Index(nameof(MessageId))]
    public class TAIChatMessageStore : CUD_User
    {
        [MaxLength(200)]
        public string Key { get; set; }

        [MaxLength(100)]

        public string ThreadId { get; set; }

        [Description("消息时间stamp")]
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// 角色标识
        /// </summary>
        [MaxLength(50)]
        public string Role { get; set; }
        public string SerializedMessage { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>

        public string? MessageText { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        [Description("消息id")]
        [MaxLength(100)] 
        public string? MessageId { get; set; }
    }
}
