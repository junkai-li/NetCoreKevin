using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 聊天记录
    /// </summary>
    [Table("TAIChatHistorys")]
    public class TAIChatHistorys : CD_User
    {
        /// <summary>
        /// 聊天用户di
        /// </summary>
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual TUser? User { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid AppId { get; set; }

        [ForeignKey("UserId")]
        public virtual TAIApps? App { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary> 
        public string Context { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        public bool IsSend { get; set; } = false;

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }
    }
}
