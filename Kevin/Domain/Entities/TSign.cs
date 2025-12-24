namespace kevin.Domain.Entities
{

    /// <summary>
    /// 点赞或标记喜欢记录表
    /// </summary>
    [Table("TSign")]
    [Description("点赞或标记喜欢记录表")]
    public partial class TSign : CD_User
    {

        /// <summary>
        /// 外链表名称
        /// </summary>
        [StringLength(100)]
        [Description("外链表名称")]
        public   string? Table { get; set; }



        /// <summary>
        /// 外链记录ID
        /// </summary>
        [Description("外链记录ID")]
        public string TableId { get; set; }



        /// <summary>
        /// 自定义标记
        /// </summary>
        [StringLength(200)]
        [Description("自定义标记")]
        public   string? Sign { get; set; }

    }
}
