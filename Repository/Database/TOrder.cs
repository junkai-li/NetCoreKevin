using Repository.Database.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{


    /// <summary>
    /// 订单表
    /// </summary>
    public class TOrder : CUD_User
    {


        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }



        /// <summary>
        /// 订单类型
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        public decimal Price { get; set; }


        /// <summary>
        /// 支付流水号
        /// </summary>
        public string SerialNo { get; set; }


        /// <summary>
        /// 订单状态
        /// </summary>
        public string State { get; set; }


        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType { get; set; }


        /// <summary>
        /// 支付状态
        /// </summary>
        public bool PayState { get; set; }


        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }



        /// <summary>
        /// 实际支付金额
        /// </summary>
        [Column(TypeName = "decimal(38,2)")]
        public decimal PayPrice { get; set; }



        /// <summary>
        /// 订单详情
        /// </summary>
        public virtual List<TOrderDetail> OrderDetails { get; set; }
    }
}
