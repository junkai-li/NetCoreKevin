using kevin.Domain.Share.Dtos.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 对话
    /// </summary>
    public class AIChatsDto : CUD_User_Dto
    {
        /// <summary>
        /// 主题--如果为空则取第一条记录当作聊天主题
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// 聊天用户di
        /// </summary>
        [Description("聊天用户Id")]
        public long UserId { get; set; }
        public virtual string? User { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Description("应用ID")]
        [Required(ErrorMessage = "应用ID不能为空")]
        public long AppId { get; set; }
        public virtual string? App { get; set; }

        /// <summary>
        /// 最后一条聊天记录
        /// </summary>
        public String LastMessage { get; set; }

        /// <summary>
        /// 对话下的所有聊天记录
        /// </summary>
        public virtual List<AIChatHistorysDto>? AIChatHistorysList { get; set; }
    }
}
