using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{

    /// <summary>
    /// 城市信息表
    /// </summary>
    public  class TRegionCity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }


        /// <summary>
        /// 城市名称
        /// </summary>
        public string City { get; set; }


        /// <summary>
        /// 所属省份ID
        /// </summary>
        public int ProvinceId { get; set; }
        public virtual TRegionProvince Province { get; set; }


        /// <summary>
        /// 城市下所有区域信息
        /// </summary>
        public virtual List<TRegionArea> TRegionArea { get; set; }
    }
}
