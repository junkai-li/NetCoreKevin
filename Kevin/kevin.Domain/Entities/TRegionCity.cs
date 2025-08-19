namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 城市信息表
    /// </summary>
    [Table("TRegionCity")]
    [Description("城市信息表")]
    public partial class TRegionCity : CD
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }


        /// <summary>
        /// 城市名称
        /// </summary>
        [StringLength(200)]
        [Description("城市名称")]
        public string City { get; set; }


        /// <summary>
        /// 所属省份ID
        /// </summary>
        [Description("所属省份ID")]
        public int ProvinceId { get; set; }
        public virtual TRegionProvince Province { get; set; }


        /// <summary>
        /// 城市下所有区域信息
        /// </summary>
        public virtual List<TRegionArea> TRegionArea { get; set; }
    }
}
