using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.Bases
{

    /// <summary>
    /// 创建，删除
    /// </summary>
    public class CD_Dto
    {

        /// <summary>
        /// 主键标识ID
        /// </summary>
        [Description("主键标识ID")]
        public long Id { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; } 
        /// <summary>
        /// 租户ID;
        /// </summary> 
        [Description("租户ID_Code")]
        public Int32 TenantId { get; set; }

    }
}
