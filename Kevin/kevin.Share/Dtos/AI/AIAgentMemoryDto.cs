using kevin.Domain.Share;
using kevin.Domain.Share.Enums;
using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 智能体记忆新增/编辑 DTO（管理端专用，对齐 AIAppsDto 惯例：字段白名单 + Check() 校验）。
    /// <para>
    /// 白名单仅含：Id / AIAppsId / Content / Keywords / MemoryType / Importance / ExpireTime；
    /// 归属三维 UserId/TenantId/AIAppsId（编辑时）、HitCount/LastHitTime 等一律不进 DTO，
    /// 由服务端强制维护（编辑时无条件保留 original 原值，AIAppsId 仅新增分支生效、编辑分支忽略）。
    /// </para>
    /// </summary>
    public class AIAgentMemoryDto
    {
        /// <summary>
        /// 记忆 Id；0 或不传表示新增，传已有 Id 表示编辑
        /// </summary>
        [Description("记忆 Id，0 表示新增，传已有 Id 表示编辑")]
        public long Id { get; set; }

        /// <summary>
        /// 归属智能体 Id；仅新增分支生效（指定记忆挂到哪个智能体，=0 时无默认智能体上下文置 0）；
        /// 编辑分支忽略此字段，服务端无条件保留原值（防止 Qdrant collection 归属错乱）
        /// </summary>
        [Description("归属智能体 Id，仅新增生效；编辑时被忽略并保留原值")]
        public long AIAppsId { get; set; }

        /// <summary>
        /// 记忆内容，最长 4000 字符（CharLimits.MemoryContent）；新增必填，编辑传空保留原值
        /// </summary>
        [Description("记忆内容，新增必填，最长 4000 字符；编辑传空保留原值")]
        public string Content { get; set; }

        /// <summary>
        /// 关键词，英文逗号分隔，最长 200 字符（CharLimits.MemoryKeywords）；编辑传空保留原值
        /// </summary>
        [Description("关键词，英文逗号分隔，最长 200 字符；编辑传空保留原值")]
        public string Keywords { get; set; }

        /// <summary>
        /// 记忆类型，枚举：preference/fact/task/decision/pitfall/skill/other；编辑传空保留原值
        /// </summary>
        [Description("记忆类型 preference/fact/task/decision/pitfall/skill/other；编辑传空保留原值")]
        public string MemoryType { get; set; }

        /// <summary>
        /// 重要程度 0-10；编辑传 0 或负值时服务端保留原值（防止前端只传 Content 时被清零）
        /// </summary>
        [Description("重要程度 0-10；编辑传 0 或负值保留原值")]
        public int Importance { get; set; }

        /// <summary>
        /// 过期时间（短期记忆 task 用）；编辑传 null 时服务端保留原值（纠偏口径下前端只传 Content）
        /// </summary>
        [Description("过期时间；编辑传 null 保留原值")]
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 入参校验（仿 AIAppsDto.Check 惯例，由服务层 AddEdit 调用）：
        /// 新增时 Content 必填；Content/Keywords 长度边界；Importance 0-10 边界；MemoryType 合法值。
        /// 空值语义（传空保留原值）在服务层处理，本方法只校验"传入值本身"的合法性。
        /// </summary>
        public void Check()
        {
            if (this.Id == default && string.IsNullOrWhiteSpace(this.Content))
            {
                throw new FieldValidationException("新增记忆时内容不能为空");
            }
            if (!string.IsNullOrWhiteSpace(this.Content) && this.Content.Trim().Length > 4000)
            {
                throw new FieldValidationException("记忆内容最长 4000 字符");
            }
            if (!string.IsNullOrWhiteSpace(this.Keywords) && this.Keywords.Trim().Length > 200)
            {
                throw new FieldValidationException("记忆关键词最长 200 字符");
            }
            if (this.Importance < 0 || this.Importance > 10)
            {
                throw new FieldValidationException("重要程度必须在 0-10 之间");
            }
            if (!string.IsNullOrWhiteSpace(this.MemoryType) && !MemoryTypes.IsValid(this.MemoryType))
            {
                throw new FieldValidationException("记忆类型必须是 preference/fact/task/decision/pitfall/skill/other 之一");
            }
        }
    }
}
