using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IAISkillToolBindIdService : IBaseService
    {

        /// <summary>
        /// 获取管理Id的所有列表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<List<TAISkillToolBindId>> GetListById(string Id);
        /// <summary>
        /// 批量绑定管理哦工具
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> BatchAddIds(string Id, List<long> ids);
        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAISkillToolBindId>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAISkillToolBindId data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
