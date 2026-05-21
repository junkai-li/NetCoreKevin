using kevin.Domain.Entities;
namespace kevin.Domain.Interfaces.IServices
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IUserInfoService : IBaseService
    {
         /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TUserInfo>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TUserInfo data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
