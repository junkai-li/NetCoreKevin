namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 文章表
    /// </summary>
    [Table("TArticle")]
    [Description("文章表")]
    public partial class TArticle : CD_User
    {

        /// <summary>
        /// 类别ID
        /// </summary>
        [Description("类别ID")]
        public Guid CategoryId { get; set; }
        public virtual TCategory Category { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [MaxLength(200)]
        public string Title { get; set; }


        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")] 
        public string Content { get; set; }


        /// <summary>
        /// 是否推荐
        /// </summary>
        [Description("是否推荐")]
        public bool IsRecommend { get; set; }


        /// <summary>
        /// 是否显示
        /// </summary>
        [Description("是否显示")]
        public bool IsDisplay { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }


        /// <summary>
        /// 点击数
        /// </summary>
        [Description("点击数")]
        public int ClickCount { get; set; }


        /// <summary>
        /// 摘要
        /// </summary>
        [Description("摘要")]
        public string Abstract { get; set; }


    }
}
