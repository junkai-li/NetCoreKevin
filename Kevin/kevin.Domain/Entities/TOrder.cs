 
namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 订单表
    /// </summary>
    [Table("TOrder")]
    [Description("订单表")]
    public partial class TOrder : CUD_User
    {


        /// <summary>
        /// 订单号
        /// </summary>
        [StringLength(100)]
        [Description("订单号")]
        public string OrderNo { get; set; }



        /// <summary>
        /// 订单类型
        /// </summary>
        [StringLength(50)]
        [Description("订单类型")]
        public string Type { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        [Description("价格")]
        public decimal Price { get; set; }


        /// <summary>
        /// 支付流水号
        /// </summary>
        [StringLength(100)]
        [Description("支付流水号")]
        public string SerialNo { get; set; }


        /// <summary>
        /// 订单状态
        /// </summary>
        [StringLength(50)]
        [Description("订单状态")]
        public string State { get; set; }


        /// <summary>
        /// 支付方式
        /// </summary>
        [StringLength(50)]
        [Description("支付方式")]
        public string PayType { get; set; }


        /// <summary>
        /// 支付状态
        /// </summary>
        [Description("支付状态")]
        public bool PayState { get; set; }


        /// <summary>
        /// 支付时间
        /// </summary>
        [Description("支付时间")]
        public DateTime? PayTime { get; set; }



        /// <summary>
        /// 实际支付金额
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        [Description("实际支付金额")]
        public decimal PayPrice { get; set; }



        /// <summary>
        /// 订单详情
        /// </summary>
        public virtual List<TOrderDetail> OrderDetails { get; set; }
    }
}
