using kevin.Domain.Entities;
using Kevin.Common.App;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 系统权限表
    /// </summary>
    [Table("TPermission")]
    [Description("系统权限表")] 
    public partial class TPermission
    {
        /// <summary>
        /// 主键标识ID
        /// </summary>
        [Description("主键标识ID")]
        [MaxLength(255)]
        public string Id { get; set; }



        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }



        /// <summary>
        /// 是否删除
        /// </summary>
        [Description("是否删除")]
        [Column(TypeName = "bit")]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public long? CreateUserId { get; set; }
        public TUser? CreateUser { get; set; }


        /// <summary>
        /// 编辑人ID
        /// </summary>
        [Description("编辑人ID")]
        public long? UpdateUserId { get; set; }
        public TUser? UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Description("更新时间")]
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        [Description("删除人ID")]
        public long? DeleteUserId { get; set; }
        public TUser? DeleteUser { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 区域;
        /// </summary>
        [StringLength(50)]
        [Description("区域")]
        public virtual string? AreaName { get; set; }
        /// <summary>
        /// 模块名;
        /// </summary>
	    [StringLength(50)]
        [Description("系统权限表")]
        public virtual string? ModuleName { get; set; }
        /// <summary>
        /// 动作名;
        /// </summary>
	    [StringLength(50)]
        [Description("动作名")]
        public virtual string? ActionName { get; set; }
        /// <summary>
        /// 模块全名;
        /// </summary>
	    [StringLength(512)]
        [Description("模块全名")]
        public virtual string? FullName { get; set; }
        /// <summary>
        /// Module;
        /// </summary>
	    [StringLength(50)]
        [Description("Module")]
        public virtual string? Module { get; set; }
        /// <summary>
        /// Action;
        /// </summary>
	    [StringLength(50)]
        [Description("Action")]
        public virtual string? Action { get; set; }
        /// <summary>
        /// 区域名称;
        /// </summary>
	    [StringLength(50)]
        [Description("区域名称")]
        public virtual string? Area { get; set; }
        /// <summary>
        /// Method;
        /// </summary>
	    [StringLength(50)]
        [Description("Method")]
        public virtual string? HttpMethod { get; set; }
        /// <summary>
        /// 手动添加;
        /// </summary>
        [Description("手动添加")]
        public virtual bool IsManual { get; set; }
        /// <summary>
        /// 序号;
        /// </summary>
        [Description("序号")]
        public virtual int? Seq { get; set; }
        /// <summary>
        /// 图标;
        /// </summary>
	    [StringLength(50)]
        [Description("图标")]
        public virtual string? Icon { get; set; }


        /// <summary>
        /// 租户id
        /// </summary>
        [Description("租户id")]
        public int TenantId { get; set; }

        /// <summary>
        /// 权限类型1.菜单权限2.功能权限3.数据权限 4.接口权限
        /// </summary>
        public Int32 PermissionType { get; set; }
    }
}
