namespace kevin.Domain.Kevin
{
    [Table("TRegionTown")]
    [Description("街道信息表")]
    public partial class TRegionTown : CD
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }



        /// <summary>
        /// 街道名称
        /// </summary>
        [StringLength(500)]
        [Description("街道名称")]
        public string? Town { get; set; }



        /// <summary>
        /// 所属区域ID
        /// </summary> 
        [Description("所属区域ID")]
        public int AreaId { get; set; }
        public virtual TRegionArea? Area { get; set; }


    }
}
