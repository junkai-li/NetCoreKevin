namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 操作标记
    /// </summary>
    [Table("TOSLog")]
    [Description("操作标记")]
    public partial class TOSLog : CD
    {
        /// <summary>
        /// 外链表名
        /// </summary>
        [StringLength(50)]
        [Description("外链表名")]
        public string? Table { get; set; }



        /// <summary>
        /// 外链表ID
        /// </summary>
        [Description("外链表ID")]
        public string TableId { get; set; }



        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(100)]
        [Description("标记")]
        public string? Sign { get; set; }



        /// <summary>
        /// 变动内容
        /// </summary> 
        [Description("变动内容")]
        public string? Content { get; set; }



        /// <summary>
        /// 操作人信息
        /// </summary>
        [Description("操作人信息")]
        public long? ActionUserId { get; set; }
        public virtual TUser? ActionUser { get; set; }



        /// <summary>
        /// 备注
        /// </summary> 
        [Description("备注")]
        public string? Remarks { get; set; }



        /// <summary>
        /// Ip地址
        /// </summary>
        [StringLength(100)]
        [Description("Ip地址")]
        public string? IpAddress { get; set; }



        /// <summary>
        ///  设备标记
        /// </summary> 
        [Description("设备标记")]
        public string? DeviceMark { get; set; }
    }
}
