using kevin.Domain.Events;

namespace kevin.Domain.Entities
{

    /// <summary>
    /// 日志表
    /// </summary>
    [Table("TLog")]
    [Description("日志表")]
    public partial class TLog : CD
    {
        public TLog(string sign, string type, string content)
        {
            this.Sign = sign;
            this.Type = type;
            this.Content = content; 
            this.AddDomainEvent(new TLogCreatedEvent(this),EventBus.EventBusEnums.Add);
        }

        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(50)]
        [Description("标记")]
        public string Sign { get; set; }


        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(50)]
        [Description("类型")]
        public string Type { get; set; }



        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string Content { get; set; }

    }
}
