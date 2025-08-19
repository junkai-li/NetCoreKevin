namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 栏目信息表
    /// </summary>
    [Table("TCategory")]
    [Description("栏目信息表")]
    public partial class TCategory : CD_User
    {


        /// <summary>
        /// 频道ID
        /// </summary>
        [Description("频道ID")]
        public Guid ChannelId { get; set; }
        public virtual TChannel Channel { get; set; }


        /// <summary>
        /// 栏目名目
        /// </summary>
        [StringLength(100)]
        [Description("栏目名目")]
        public string Name { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }


        /// <summary>
        /// 父级栏目ID
        /// </summary>
        [Description("父级栏目ID")]
        public Guid? ParentId { get; set; }
        public virtual TCategory Parent { get; set; }

        [Description("备注")]
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }




        public virtual List<TArticle> TArticle { get; set; }
    }
}
