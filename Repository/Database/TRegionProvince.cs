using Repository.Bases;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{

    /// <summary>
    /// 省份信息表
    /// </summary>
    public class TRegionProvince : CD
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }



        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }



        /// <summary>
        /// 省份下包含的所有城市信息
        /// </summary>
        public virtual List<TRegionCity> TRegionCity { get; set; }

    }
}
