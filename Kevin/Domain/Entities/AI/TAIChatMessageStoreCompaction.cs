using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 专门用于存储AI聊天记录压缩后的表
    /// </summary>
    [Table("TAIChatMessageStoreCompaction")]
    [Description("专门用于存储AI聊天记录压缩后的表")] 
    [Index(nameof(ThreadId))] 
    public class TAIChatMessageStoreCompaction : CUD
    {
        [MaxLength(150)]
        public string ThreadId { get; set; } = "";
        /// <summary>
        /// 压缩消息内容
        /// </summary>

        public string CompactionMessageText { get; set; } = "";

        /// <summary>
        /// 压缩消息结果
        /// </summary>

        public string CompactionResultMessageText { get; set; } = "";
    }
}
