using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 历史聊天压缩记录服务
    /// </summary>
    public interface IAIChatMessageStoreCompactionService : IBaseService
    {
         /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAIChatMessageStoreCompaction>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 获取历史聊天压缩记录
        /// </summary>
        /// <returns></returns>
        Task<List<TAIChatMessageStoreCompaction>> GetList(string threadId);

        /// <summary>
        /// //获取历史聊天压缩记录 提示词
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        Task<String> GetThreadPrompt(string threadId);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAIChatMessageStoreCompaction data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);

        /// <summary>
        ///获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话,压缩提取摘要版本
        /// </summary> 
        /// <returns></returns>
        [Description("获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话(压缩提取摘要版本)")]
        Task<String> GetAIToolThreadPrompt();
    }
}
