namespace Repository.Database
{
    public class TRegionTown : CD
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }



        /// <summary>
        /// 街道名称
        /// </summary>
        [StringLength(500)]
        public string Town { get; set; }



        /// <summary>
        /// 所属区域ID
        /// </summary>
        public int AreaId { get; set; }
        public virtual TRegionArea Area { get; set; }


    }
}
