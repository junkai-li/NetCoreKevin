using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI对话
    /// </summary>
    [Table("TAIChats")]
    [Description("AI对话记录")]
    public class TAIChats : CUD_User
    {
        /// <summary>
        /// 主题
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; } 

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
        /// 对话下的所有聊天记录
        /// </summary>
        public virtual List<TAIChatHistorys>? TAIChatHistorys { get; set; }
    }
}
