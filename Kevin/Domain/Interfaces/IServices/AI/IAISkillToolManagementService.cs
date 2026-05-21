using kevin.Domain.Entities.AI;
using kevin.Domain.Share.Dtos.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IAISkillToolManagementService : IBaseService
    {
         /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<AISkillToolManagementDto>> GetPageData(dtoPagePar<int> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(AISkillToolManagementDto data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
