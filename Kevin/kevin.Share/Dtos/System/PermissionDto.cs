namespace kevin.Permission.Permisson
{
    public class PermissionDto
    {
        /// <summary>
        /// 主键标识ID
        /// </summary> 
        public string Id { get; set; } 

        /// <summary>
        /// 创建时间
        /// </summary> 
        public DateTime CreateTime { get; set; } 

        /// <summary>
        /// 创建人ID
        /// </summary> 
        public Guid? CreateUserId { get; set; } 


        /// <summary>
        /// 编辑人ID
        /// </summary> 
        public Guid? UpdateUserId { get; set; } 

        /// <summary>
        /// 更新时间
        /// </summary> 
        public DateTime? UpdatedTime { get; set; }
         
        /// <summary>
        /// 区域;
        /// </summary> 
        public virtual string? AreaName { get; set; }
        /// <summary>
        /// 模块名;
        /// </summary> 
        public virtual string? ModuleName { get; set; }
        /// <summary>
        /// 动作名;
        /// </summary> 
        public virtual string? ActionName { get; set; }
        /// <summary>
        /// 模块全名;
        /// </summary> 
        public virtual string? FullName { get; set; }
        /// <summary>
        /// Module;
        /// </summary> 
        public virtual string? Module { get; set; }
        /// <summary>
        /// Action;
        /// </summary> 
        public virtual string? Action { get; set; }
        /// <summary>
        /// 区域名称;
        /// </summary> 
        public virtual string? Area { get; set; }
        /// <summary>
        /// Method;
        /// </summary> 
        public virtual string? HttpMethod { get; set; }
        /// <summary>
        /// 手动添加;
        /// </summary> 
        public virtual bool? IsManual { get; set; }
        /// <summary>
        /// 序号;
        /// </summary> 
        public virtual int? Seq { get; set; }
        /// <summary>
        /// 图标;
        /// </summary> 
        public virtual string? Icon { get; set; }

        /// <summary>
        /// 权限类型1.菜单权限2.功能权限3.数据权限 4.接口权限
        /// </summary>
        public Int32 permission_type { get; set; } 

        /// <summary>
        /// 租户id
        /// </summary>
        public int TenantId { get; set; }

    }
}
