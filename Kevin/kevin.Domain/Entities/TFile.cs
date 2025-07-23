namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 文件表
    /// </summary>
    public class TFile : CD_User
    {


        /// <summary>
        /// 文件名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }


        /// <summary>
        /// 保存路径
        /// </summary>
        [StringLength(200)]
        public string Path { get; set; }


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
        [StringLength(50)]
        public string Sign { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
