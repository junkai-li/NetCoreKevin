using kevin.Domain.Entities;
namespace kevin.Domain.Interfaces.IServices
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IRolePermissionService : IBaseService
    {
         /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TRolePermission>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TRolePermission data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
