using kevin.Domain.Entities;
namespace kevin.Domain.Interfaces.IServices
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IRegionCityService : IBaseService
    {
         /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TRegionCity>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TRegionCity data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
