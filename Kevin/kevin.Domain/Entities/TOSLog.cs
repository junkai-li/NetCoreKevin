namespace kevin.Domain.Kevin
{
    public class TOSLog : CD
    {
        /// <summary>
        /// 外链表名
        /// </summary>
        [StringLength(50)]
        public string Table { get; set; }



        /// <summary>
        /// 外链表ID
        /// </summary>
        public Guid TableId { get; set; }



        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(100)]
        public string Sign { get; set; }



        /// <summary>
        /// 变动内容
        /// </summary> 
        public string Content { get; set; }



        /// <summary>
        /// 操作人信息
        /// </summary>
        public Guid? ActionUserId { get; set; }
        public virtual TUser ActionUser { get; set; }



        /// <summary>
        /// 备注
        /// </summary> 
        public string Remarks { get; set; }



        /// <summary>
        /// Ip地址
        /// </summary>
        [StringLength(100)]
        public string IpAddress { get; set; }



        /// <summary>
        ///  设备标记
        /// </summary>
        [StringLength(100)]
        public string DeviceMark { get; set; }
    }
}
