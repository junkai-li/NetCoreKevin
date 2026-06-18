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
    }
}
