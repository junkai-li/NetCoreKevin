using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.System
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    public class MsgGetPageDataParDto
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType SysMsgType { get; set; }= MessageType.All;

        /// <summary>
        /// 相关人id
        /// </summary>
        public string  UserId { get; set; } = "";  

        /// <summary>
        /// 关联Id
        /// </summary>
        public string? AssociatedId { get; set; } = "";
        public string? AssociatedTable { get; set; } = "";

        /// <summary>
        ///  1.已读 2.未读 3.所有消息
        /// </summary>
        public int IsRead { get; set; }
    }
}
