namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI提示词配置
    /// </summary>
    [Table("TAIPrompts")]
    [Description("AI提示词配置")]
    public partial class TAIPrompts : CUD_User
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Description("名称")]
        [StringLength(100)]
        public String Name { get; set; } = "";

        /// <summary>
        /// 提示词
        /// </summary>
        [Description("提示词")]
        [Required] 
        public String Prompt { get; set; } = "";
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        [Description("描述")]
        public String? Description { get; set; }
    }
}
