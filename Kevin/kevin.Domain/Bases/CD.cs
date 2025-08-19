using kevin.Domain.EventBus;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kevin.Domain.Bases
{

    /// <summary>
    /// 创建，删除
    /// </summary>
    public class CD: DEBaseEntity
    {

        /// <summary>
        /// 主键标识ID
        /// </summary>
        [Description("主键标识ID")]
        public Guid Id { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }



        /// <summary>
        /// 是否删除
        /// </summary>
        [Description("是否删除")]
        public bool IsDelete { get; set; }



        /// <summary>
        /// 删除时间
        /// </summary>
        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 行版本标记
        /// </summary>
        /// <remarks>通用的RowVersion</remarks>
        [ConcurrencyCheck]
        [Description("行版本标记")]
        public Guid? RowVersion { get; set; }




        /// <summary>
        /// 行版本标记
        /// </summary>
        /// <remarks>PostgreSql的RowVersion</remarks>
        [Description("行版本标记")]
        public uint xmin { get; set; }



        ///// <summary>
        ///// 行版本标记
        ///// </summary>
        ///// <remarks>SqlServer的RowVersion</remarks>
        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        /// <summary>
        /// 租户ID;
        /// </summary> 
        [Description("租户ID_Code")]
        public Int32 TenantId { get; set; }

    }
}
