

namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 产品表
    /// </summary>
    [Table("TProduct")]
    [Description("产品表")]
    public partial class TProduct : CUD_User
    {

        /// <summary>
        /// SKU
        /// </summary>
        [StringLength(200)]
        [Description("SKU")]
        public string SKU { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(200)]
        [Description("名称")]
        public string Name { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        [Description("价格")]
        public decimal Price { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(200)]
        [Description("描述")]
        public string Detail { get; set; }

    }
}
