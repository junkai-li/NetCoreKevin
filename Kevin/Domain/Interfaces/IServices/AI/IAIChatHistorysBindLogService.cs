using kevin.Domain.Entities.AI;
using kevin.Domain.Share.Dtos.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    ///  
    /// </summary>
    public interface IAIChatHistorysBindLogService : IBaseService
    {
         /// <summary>
        ///  
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAIChatHistorysBindLog>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="aIChatHistorysIds"></param>
        /// <returns></returns>
        Task<List<AIChatHistorysBindLogDto>> GetByIds(List<long> aIChatHistorysIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAIChatHistorysBindLog data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
