namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 系统权限表
    /// </summary>
    [Table("TPermission")]
    public partial class TPermission
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
        /// 是否删除
        /// </summary>
        [Column(TypeName = "bit")]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid? CreateUserId { get; set; }
        public TUser CreateUser { get; set; }


        /// <summary>
        /// 编辑人ID
        /// </summary>
        public Guid? UpdateUserId { get; set; }
        public TUser UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        public Guid? DeleteUserId { get; set; }
        public TUser DeleteUser { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 区域;
        /// </summary>
        [StringLength(50)]
        public virtual string AreaName { get; set; }
        /// <summary>
        /// 模块名;
        /// </summary>
	    [StringLength(50)]
        public virtual string ModuleName { get; set; }
        /// <summary>
        /// 动作名;
        /// </summary>
	    [StringLength(50)]
        public virtual string ActionName { get; set; }
        /// <summary>
        /// 模块全名;
        /// </summary>
	    [StringLength(512)]
        public virtual string FullName { get; set; }
        /// <summary>
        /// Module;
        /// </summary>
	    [StringLength(50)]
        public virtual string Module { get; set; }
        /// <summary>
        /// Action;
        /// </summary>
	    [StringLength(50)]
        public virtual string Action { get; set; }
        /// <summary>
        /// 区域名称;
        /// </summary>
	    [StringLength(50)]
        public virtual string Area { get; set; }
        /// <summary>
        /// Method;
        /// </summary>
	    [StringLength(50)]
        public virtual string HttpMethod { get; set; }
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
	    [StringLength(50)]
        public virtual string Icon { get; set; }
    }
}
