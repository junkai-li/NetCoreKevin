namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI智能体技能工具绑定
    /// </summary>
    [Table("TAISkillToolBindId")]
    [Description("AI智能体技能工具绑定")]
    public class TAISkillToolBindId : CUD_User
    {
        /// <summary>
        /// AI智能体技能Id
        /// </summary>
        [Description("AISkillToolManagementId")]
        public long AISkillToolManagementId { get; set; }
        public virtual TAISkillToolManagement? AISkillToolManagement { get; set; }

        /// <summary>
        /// 关联Id （TAIApps Id）（TUser Id）  TRole Id
        /// </summary>
        [StringLength(100)]
        [Description(" 关联Id （TAIApps Id）（TUser Id）  TRole Id")]
        public string BindId { get; set; } = "";
    }
}
