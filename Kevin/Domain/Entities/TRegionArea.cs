namespace kevin.Domain.Entities
{

    /// <summary>
    /// 区域信息表
    /// </summary>
    [Table("TRegionArea")]
    [Description("区域信息表")]
    public partial class TRegionArea : CD
    {


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Description("Id")]
        public new int Id { get; set; }



        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(200)]
        [Description("区域名称")]
        public string? Area { get; set; }



        /// <summary>
        /// 所属城市ID
        /// </summary>
        [Description("所属城市ID")]
        public int CityId { get; set; }
        public virtual TRegionCity? City { get; set; }



        /// <summary>
        /// 城市下所有乡镇信息
        /// </summary>
        public virtual List<TRegionTown>? TRegionTown { get; set; }


    }
}
