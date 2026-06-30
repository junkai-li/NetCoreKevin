using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAIJsonLog
    /// </summary>
    [Table("TAIJsonLog")]
    [Description("AI保存相关json日志")]
    public class TAIJsonLog : CUD
    {
        /// <summary>
        /// 对话id
        /// </summary>
        [Description("对话id")]
        public long AIChatsId { get; set; }

        /// <summary>
        /// AI智能体Id
        /// </summary>
        [Description("AI智能体Id")]
        public long AIAppsId { get; set; }
        /// <summary>
        /// Json
        /// </summary>
        [Description("Json")]
        public string Json { get; set; } = "";
    }
}
