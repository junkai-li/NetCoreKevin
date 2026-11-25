namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 聊天本轮覆盖上下文（Per-Round Override）：随单条消息入参透传、不落库的临时挂载配置
    /// （仿 <see cref="AIChatHistorysDto.AgentMode"/> / <see cref="AIChatHistorysDto.ExternalTools"/> 的“仅本轮入参”模式）。
    /// <para>
    /// 由 <see cref="AIChatHistorysDto"/> 的新增字段经 <see cref="From"/> 构造后透传至
    /// <c>IAIAppsService.GetAppAIAgentOptions</c>，仅影响本轮知识库检索工具与 Skill 的挂载集合；
    /// 下一条消息不携带时回退智能体默认绑定。未来聊天室 / OpenAI 兼容接口可复用。
    /// </para>
    /// </summary>
    public class AIPerRoundOverride
    {
        /// <summary>本轮追加挂载的知识库 Id 列表（已解析、去重）</summary>
        public List<long> SelectedKmsIds { get; set; } = new();

        /// <summary>本轮追加挂载的 Skill Id 列表（已解析、去重）</summary>
        public List<long> SelectedSkillIds { get; set; } = new();

        /// <summary>本轮追加挂载的 MCP Id 列表（已解析、去重）</summary>
        public List<long> SelectedMcpIds { get; set; } = new();

        /// <summary>忽略智能体默认绑定知识库 TAIApps.KmsId</summary>
        public bool ExcludeDefaultKmss { get; set; }

        /// <summary>忽略智能体默认绑定 Skill</summary>
        public bool ExcludeDefaultSkills { get; set; }

        /// <summary>
        /// 从聊天入参构造本轮覆盖上下文：字符串 Id 逐个 long.TryParse（非法/非正数忽略）并去重；
        /// 源字段均为空/默认时返回等价于空的 override（旧客户端不传新字段 → 行为与现状一致）。
        /// </summary>
        public static AIPerRoundOverride From(AIChatHistorysDto par)
        {
            if (par == null) return new AIPerRoundOverride();
            return new AIPerRoundOverride
            {
                SelectedKmsIds = ParseIds(par.SelectedKmsIds),
                SelectedSkillIds = ParseIds(par.SelectedSkillIds),
                SelectedMcpIds = ParseIds(par.SelectedMcpIds),
                ExcludeDefaultKmss = par.ExcludeDefaultKmss,
                ExcludeDefaultSkills = par.ExcludeDefaultSkills
            };
        }

        /// <summary>字符串 Id 解析：非法或非正数忽略，结果去重保序</summary>
        private static List<long> ParseIds(List<string>? ids)
        {
            var result = new List<long>();
            if (ids is not { Count: > 0 }) return result;
            var seen = new HashSet<long>();
            foreach (var raw in ids)
            {
                if (long.TryParse(raw, out var id) && id > 0 && seen.Add(id))
                {
                    result.Add(id);
                }
            }
            return result;
        }
    }
}
