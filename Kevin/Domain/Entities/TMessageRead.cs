using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 消息已读表
    /// </summary>
    [Table("TMessageRead")]
    [Description("消息已读表")]
    public class TMessageRead : CD_User
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public virtual TMessage Message { get; set; }
    }
}
