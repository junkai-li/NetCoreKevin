using kevin.Domain.Share.Dtos.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// SkillToolManagement
    /// </summary>
    public interface IAISkillToolManagementService : IBaseService
    {
        /// <summary>
        ///   获取所有Skill技能
        /// </summary> 
        /// <returns></returns>
        Task<List<AISkillToolManagementDto>> GetAllSkills();

        /// <summary>
        /// 获取所有mcp接口
        /// </summary>
        /// <returns></returns>
        Task<List<AISkillToolManagementDto>> GetAllMcps();

        /// <summary>
        /// 根据Id获取Skill技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AISkillToolManagementDto> GetById(long id);
        /// <summary>
        ///   获取所有Tool工具
        /// </summary> 
        /// <returns></returns>
        Task<List<AISkillToolManagementDto>> GetAllTools();
        /// <summary>
        ///SkillToolManagement 分页获取
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<AISkillToolManagementDto>> GetPageData(dtoPagePar<int> dtoPagePar);

        /// <summary>
        /// 新增编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(AISkillToolManagementDto data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);

        Task<List<AISkillToolManagementDto>> GetNotDataPerAllSkills();

        /// <summary>
        /// 受数据权限控制的按 Id 批量取 Skill（isDataPer: true 隐式校验归属）：
        /// 请求 Id 中已删除/无权限/非 Skill 类型/未启用的不会出现在返回结果中（调用方按差集记 Warn 日志跳过，不阻断主流程）。
        /// </summary>
        /// <param name="ids">要加载的 Skill Id 集合（空集合直接返回空列表）</param>
        /// <returns>命中的 Skill 列表</returns>
        Task<List<AISkillToolManagementDto>> GetDataPerSkillsByIds(List<long> ids);

        /// <summary>
        /// 按 Id 加载本轮追加挂载的 MCP（isDataPer 数据权限 + Mcp 类型 + Active）：
        /// 传进来别人的/已删除/未启用的 Id 自然查不到，调用方跳过挂载即可，实现"隐式归属校验"。
        /// 写法仿 <see cref="GetDataPerSkillsByIds"/>，仅 Skill 类型换为 Mcp 类型。
        /// </summary>
        /// <param name="ids">要加载的 MCP Id 集合（空集合直接返回空列表）</param>
        /// <returns>命中的 MCP 列表</returns>
        Task<List<AISkillToolManagementDto>> GetDataPerMcpsByIds(List<long> ids);

        Task<List<AISkillToolManagementDto>> GetNotDataPerAllTools();

        Task<List<AISkillToolManagementDto>> GetNotDataPerAllMcps();

        /// <summary>
        /// 测试Mcp连接并返回该Mcp服务下的全部工具
        /// </summary>
        /// <param name="data">Mcp配置（McpType/McpUrl/McpHeaders/McpCommand/McpArguments/McpEnvironment）</param>
        /// <returns>连接成功返回工具列表，失败抛出异常</returns>
        Task<List<McpToolDto>> TestMcpConnection(McpConnectionTestDto data);

    }
}
