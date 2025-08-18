

namespace kevin.Domain.Kevin
{
    [Table("TProduct")]

    /// <summary>
    /// 产品表
    /// </summary>
    public partial class TProduct : CUD_User
    {

        /// <summary>
        /// SKU
        /// </summary>
        [StringLength(200)]
        public string SKU { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(200)]
        public string Name { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        public decimal Price { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(200)]
        public string Detail { get; set; }

    }
}
