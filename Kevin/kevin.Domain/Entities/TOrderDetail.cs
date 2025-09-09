namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 订单详情表
    /// </summary>
    [Table("TOrderDetail")]
    [Description("订单详情表")]
    public partial class TOrderDetail : CD
    {


        /// <summary>
        /// 订单ID
        /// </summary>
        [Description("订单ID")]
        public Guid OrderId { get; set; }
        public virtual TOrder? Order { get; set; }


        /// <summary>
        /// 产品ID
        /// </summary>
        [Description("产品ID")]
        public Guid ProductId { get; set; }
        public virtual TProduct? Product {get;set;}


        /// <summary>
        /// 产品数量
        /// </summary>
        [Description("产品数量")]
        public int Number { get; set; }

    }
}
