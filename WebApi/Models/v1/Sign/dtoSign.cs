using System;

namespace WebApi.Models.v1.Sign
{

    /// <summary>
    /// 标记喜欢方法入参
    /// </summary>
    public class dtoSign
    {

        /// <summary>
        /// 表名
        /// </summary>
        public string Table { get; set; }


        /// <summary>
        /// 记录ID
        /// </summary>
        public Guid TableId { get; set; }


        /// <summary>
        /// 标记
        /// </summary>
        public string Sign { get; set; }
    }
}
