namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 智能体用户角色授权绑定-用于权限管理
    /// </summary>
    [Table("TAIAppsBindId")]
    [Description("智能体用户角色授权绑定")]
    public class TAIAppsBindId : CUD_User
    {
        /// <summary>
        /// AI智能体Id
        /// </summary>
        [Description("TAIAppsId")]
        public long TAIAppsId { get; set; }
        public virtual TAIApps? TAIApps { get; set; }

        /// <summary>
        /// 关联Id （TUser Id）（TRole Id）
        /// </summary>
        [StringLength(100)]
        [Description(" 关联Id  （TUser Id）  TRole Id")]
        public string BindId { get; set; } = "";
    }
}
