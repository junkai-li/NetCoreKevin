using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IAIJsonLogService : IBaseService, IBaseAIToolService
    {
        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAIJsonLog>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        [Description("专门用于保存 Json 数据。")]
        Task<string> Add([Description("传入完整的json结构数据比如 {\"name\":\"kevin\"}")][Required] string Json);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAIJsonLog data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
