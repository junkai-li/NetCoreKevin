using kevin.Domain.Share.Dtos.AI;
using Microsoft.Agents.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIAppsService : IBaseService
    {
        /// <summary>
        /// /分页查询智能体
        /// </summary>
        /// <param name="dtoPage"></param>
        /// <returns></returns>
        Task<dtoPageData<AIAppsDto>> GetPageData(dtoPagePar<string> dtoPage);
        /// <summary>
        /// 获取智能体详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AIAppsDto> GetDetails(long id);
        Task<AIAppsDto> GetNoPerDetails(long id);

        /// <summary>
        /// 获取智能体AI配置
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="aIPrompts"></param>
        /// <param name="systemPrompt"></param>
        /// <param name="par"></param>
        /// <param name="parAi"></param>
        /// <returns></returns>
        Task<ChatClientAgentOptions> GetAppAIAgentOptions(AIAppsDto aiapp, AIPromptsDto aIPrompts, string systemPrompt, AIChatHistorysDto par, object parAi, CancellationToken cancellationToken = default);

        /// <summary>
        /// 新增初始化
        /// </summary>
        /// <returns></returns>
        Task<AIAppsDto> NewInitialization();
        Task<List<AIAppsDto>> GetALLList();
        /// <summary>
        /// 新增或编辑智能体
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        Task<bool> AddEdit(AIAppsDto par);
        /// <summary>
        /// 删除智能体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);

        /// <summary>
        ///  获取我有权限的所有智能体
        /// </summary>
        /// <returns></returns>
        Task<List<AIAppsDto>> GetMyALLList(); 
    }
}
