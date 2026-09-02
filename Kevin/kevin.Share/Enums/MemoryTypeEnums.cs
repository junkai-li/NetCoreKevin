namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// 智能体长期记忆类型常量与校验
    /// </summary>
    /// <remarks>
    /// 数据库 TAIAgentMemory.MemoryType 以字符串存储，本类提供合法值集合与校验方法。
    /// 保留原有 4 种（preference/fact/task/other）向后兼容，新增 3 种（decision/pitfall/skill）提升分类精度。
    /// </remarks>
    public static class MemoryTypes
    {
        /// <summary>用户偏好：沟通风格、行为习惯、长期约束（如"回答用中文"、"先给方案再动手"）</summary>
        public const string Preference = "preference";

        /// <summary>事实：项目技术栈、架构、配置、模块边界等客观事实（如"项目用 .NET 9 + Vue 3"）</summary>
        public const string Fact = "fact";

        /// <summary>任务：临时任务上下文、待办事项、阶段性目标（如"本次重构目标是解耦 X 模块"）</summary>
        public const string Task = "task";

        /// <summary>决策：重要设计/架构决策，含结论 + 权衡 + 拒绝的替代方案 + 适用/失效条件</summary>
        public const string Decision = "decision";

        /// <summary>踩坑教训：Bug 类 + 根因 + 修复模式 + 可复用教训</summary>
        public const string Pitfall = "pitfall";

        /// <summary>技能经验：工具/框架使用技巧，含场景 + 方法 + 注意事项</summary>
        public const string Skill = "skill";

        /// <summary>其他：确实无法归入以上 6 类时才用</summary>
        public const string Other = "other";

        /// <summary>
        /// 全部合法记忆类型（小写）
        /// </summary>
        public static readonly string[] All = new[]
        {
            Preference, Fact, Task, Decision, Pitfall, Skill, Other
        };

        /// <summary>
        /// 校验记忆类型是否合法（不区分大小写）
        /// </summary>
        /// <param name="memoryType">待校验的记忆类型字符串</param>
        /// <returns>合法返回 true，否则 false</returns>
        public static bool IsValid(string? memoryType)
        {
            if (string.IsNullOrWhiteSpace(memoryType)) return false;
            var normalized = memoryType.Trim().ToLowerInvariant();
            return All.Contains(normalized);
        }

        /// <summary>
        /// 规范化记忆类型：去空格 + 转小写；非法值回退为 <see cref="Other"/>
        /// </summary>
        /// <param name="memoryType">原始记忆类型字符串</param>
        /// <returns>规范化后的合法记忆类型</returns>
        public static string Normalize(string? memoryType)
        {
            if (string.IsNullOrWhiteSpace(memoryType)) return Other;
            var normalized = memoryType.Trim().ToLowerInvariant();
            return All.Contains(normalized) ? normalized : Other;
        }

        /// <summary>
        /// 获取全部合法类型的描述字符串，用于接口 [Description] 或提示词拼接
        /// </summary>
        /// <returns>形如 "preference偏好/fact事实/task任务/decision决策/pitfall踩坑/skill技能/other其他"</returns>
        public static string GetDescriptionText()
        {
            return $"{Preference}偏好/{Fact}事实/{Task}任务/{Decision}决策/{Pitfall}踩坑/{Skill}技能/{Other}其他";
        }
    }
}
