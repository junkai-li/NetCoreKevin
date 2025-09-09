﻿namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 省份信息表
    /// </summary>
    [Table("TRegionProvince")]
    [Description("省份信息表")]
    public partial class TRegionProvince : CD
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }



        /// <summary>
        /// 省份
        /// </summary>
        [StringLength(200)]
        [Description("省份")]
        public string? Province { get; set; }



        /// <summary>
        /// 省份下包含的所有城市信息
        /// </summary>
        public virtual List<TRegionCity>? TRegionCity { get; set; }

    }
}
