using kevin.Domain.Share.Dtos.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 聊天记录
    /// </summary>
    public class AIChatHistorysDto : CD_User_Dto
    {
        /// <summary>
        /// 对话id
        /// </summary>
        public long AIChatsId { get; set; }
        public virtual string? AIChats { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary> 
        [Description("消息内容")]
        public String Content { get; set; } = "";

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
