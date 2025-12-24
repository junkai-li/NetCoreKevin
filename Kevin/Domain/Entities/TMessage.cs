using kevin.Domain.Share.Enums;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 消息表
    /// </summary>
    [Table("TMessage")]
    [Description("消息表")]
    public class TMessage : CUD_User
    {

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
        [MaxLength(100)]
        public string SendUserId { get; set; } = "";
        [MaxLength(100)]
        public string SendUserName { get; set; } = "";

        /// <summary>
        /// 接收人信息
        /// </summary>
         [MaxLength(100)]
        public string? RecipientUserId { get; set; } = "";
        [MaxLength(100)]
        public string? RecipientUserName { get; set; } = "";

        /// <summary>
        /// 关联Id
        /// </summary>
        [MaxLength(100)]
        public string? AssociatedId { get; set; } = "";
        [MaxLength(100)]
        public string? AssociatedTable { get; set; } = "";

    }
}
