using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 聊天对话相关日志
    /// </summary> 
    [Table("TAIChatHistorysBindLog")]
    [Description("AI聊天记录相关日志")]
    public class TAIChatHistorysBindLog : CD_User
    {
        /// <summary>
        /// 对话记录id
        /// </summary>
        public long AIChatHistorysId { get; set; } 

        /// <summary>
        /// 日志类型
        /// </summary>
        [Description("日志类型 1.知识库，2.网络搜索，3.系统提示词，文件内容 ")]
        public AIChatHistorysBindLogEnums LogType { get; set; }

        /// <summary>
        /// Log内容
        /// </summary>
        [Description("Log内容")]
        public String LogContent { get; set; } = "";

    }
}
