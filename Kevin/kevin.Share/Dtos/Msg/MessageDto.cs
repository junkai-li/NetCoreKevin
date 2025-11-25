using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.Msg
{
    public class MessageDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; }
        public string? CreateUser { get; set; }

        /// <summary>
        /// 编辑人ID
        /// </summary>
        [Description("编辑人ID")]
        public Guid? UpdateUserId { get; set; }
        public string? UpdateUser { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; } = "";

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { get; set; } = "";

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType SysMsgType { get; set; }

        /// <summary>
        /// 发送人信息
        /// </summary>
        public string SendUserId { get; set; } = "";
        public string SendUserName { get; set; } = "";

        /// <summary>
        /// 接收人信息
        /// </summary>
        public string? RecipientUserId { get; set; } = "";
        public string? RecipientUserName { get; set; } = "";

        /// <summary>
        /// 关联Id
        /// </summary>
        public string? AssociatedId { get; set; } = "";
        public string? AssociatedTable { get; set; } = "";

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// 相关附件
        /// </summary>
        public List<FileDto> Files { get; set; } = new List<FileDto>();
    }
}
