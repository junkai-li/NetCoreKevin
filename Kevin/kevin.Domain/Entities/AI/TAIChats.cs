using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI会话
    /// </summary>
    [Table("TAIChats")]
    public partial class TAIChats : CUD_User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary> 
        public string Context { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        public bool IsSend { get; set; } = false;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }
    }
}
