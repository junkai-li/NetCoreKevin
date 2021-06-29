using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{
    public class TRegionTown
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }


        /// <summary>
        /// 街道名称
        /// </summary>
        public string Town { get; set; }


        /// <summary>
        /// 所属区域ID
        /// </summary>
        public int AreaId { get; set; }
        public virtual TRegionArea Area { get; set; }

    }
}
