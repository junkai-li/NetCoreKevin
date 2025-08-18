namespace kevin.Domain.Kevin
{
    [Table("TSign")]
    /// <summary>
    /// 点赞或标记喜欢记录表
    /// </summary>
    public partial class TSign : CD_User
    {

        /// <summary>
        /// 外链表名称
        /// </summary>
        [StringLength(100)]
        public string Table { get; set; }



        /// <summary>
        /// 外链记录ID
        /// </summary>
        public Guid TableId { get; set; }



        /// <summary>
        /// 自定义标记
        /// </summary>
        [StringLength(200)]
        public string Sign { get; set; }

    }
}
