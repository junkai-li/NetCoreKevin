namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// OpenAI 协议兼容接口的用户级 API Key 配置表
    /// <para>
    /// 每个用户（UserId + TenantId）至多一条记录：Key 落库后默认 AES 加密，
    /// 前端展示仅给出脱敏片段 + 明文需手动点开查看。
    /// </para>
    /// </summary>
    [Table("TAIOpenAICompatibleKey")]
    [Description("OpenAI兼容接口的用户APIKey")]
    public partial class TAIOpenAICompatibleKey : CUD
    {
        /// <summary>
        /// Key 绑定人（雪花 Id）
        /// </summary>
        [Description("Key绑定用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// AES 加密后的 Key 全文（Base64）。外部调用时由网关层解密匹配，不在数据库中暴露明文。
        /// </summary>
        [Required]
        [MaxLength(500)]
        [Description("加密后的Key全文")]
        public string EncryptedKey { get; set; } = "";

        /// <summary>
        /// Key 前缀（脱敏展示用，如 "sk-kv"）
        /// </summary>
        [MaxLength(30)]
        [Description("Key前缀")]
        public string KeyPrefix { get; set; } = "sk-kv";

        /// <summary>
        /// Key 末尾片段（脱敏展示用，4-6 位）
        /// </summary>
        [MaxLength(10)]
        [Description("Key末尾片段")]
        public string KeySuffix { get; set; } = "";

        /// <summary>
        /// 用户备注名
        /// </summary>
        [MaxLength(100)]
        [Description("备注名")]
        public string? Name { get; set; }

        /// <summary>
        /// 是否启用（禁用后外部调用立即 401）
        /// </summary>
        [DefaultValue(true)]
        [Description("是否启用")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 最近一次被网关命中时间（用于前端展示活跃度）
        /// </summary>
        [Description("最近使用时间")]
        public DateTime? LastUsedTime { get; set; }
    }
}