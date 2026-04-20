using kevin.Domain.Share.Enums;
using Web.Global.Exceptions;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 租户表
    /// </summary>
    [Table("TTenant")]
    [Description("租户表")]
    public partial class TTenant : CUD
    {
        public TTenant(Int32 code, string name, DateTime createTime)
        {
            this.Code = code;
            this.Name = name;
            this.Status = TenantStatusEnums.Active;
            this.CreateTime = createTime;
            this.IsDelete = false;
        }

        /// <summary>
        /// 租户编码
        /// </summary>
        [Description("租户编码")]
        public Int32 Code { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        [Description("租户名称")]
        public string Name { get; set; }

        /// <summary>
        /// 租户状态
        /// </summary>
        [Description("租户状态")]
        public TenantStatusEnums Status { get; set; } = TenantStatusEnums.Active;

        /// <summary>
        /// 租户可用校验
        /// </summary>
        /// <returns></returns>
        public void IsInactiveCheck()
        {
            if (this.IsDelete)
            {
                throw new UserFriendlyException("租户已删除");
            }
            if (this.Status == kevin.Domain.Share.Enums.TenantStatusEnums.Inactive)
            {
                throw new UserFriendlyException("租户已失效");
            }
        }
    }
}
