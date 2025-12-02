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
    [Description("AI聊天记录")]
    public class TAIChatHistorys : CD_User
    {
        /// <summary>
        /// 聊天用户di
        /// </summary>
        [Description("聊天用户Id")]
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual TUser? User { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Description("应用ID")]
        public long AppId { get; set; }

        [ForeignKey("UserId")]
        public virtual TAIApps? App { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary> 
        [Description("消息内容")]
        public String Context { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        [Description("发送是true  接收是false")]
        public Boolean IsSend { get; set; } = false;

        /// <summary>
        /// 文件名
        /// </summary>
        [Description("文件名")]
        public String? FileName { get; set; }
    }
}
