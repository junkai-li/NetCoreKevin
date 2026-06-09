using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIChatHistorysBindLogDto : CD_User_Dto
    {
        /// <summary>
        /// 对话记录id
        /// </summary>
        public long AIChatHistorysId { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Description("日志类型 1.知识库，2.网络搜索，3.系统提示词，4.文件内容 ")]
        public int LogType { get; set; }

        /// <summary>
        /// Log内容
        /// </summary>
        [Description("Log内容")]
        public String LogContent { get; set; } = "";
    }
}
