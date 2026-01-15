using kevin.Domain.Entities.AI;
using kevin.Domain.Share.Dtos.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 知识库服务接口
    /// </summary>
    public interface IAIKmssService : IBaseService
    {
        /// <summary>
        ///查询数据
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<AIKmssDto>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 查询所有可用知识库
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageList<AIKmssDto>> GetList(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AIKmssDto> GetDetails(long id);

        /// <summary>
        /// 新增and编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(AIKmssDto data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);

        /// <summary>
        /// 处理上传知识库矢量数据库
        /// </summary>
        /// <param name="KmsId"></param>
        /// <returns></returns>
        Task<bool> ProcessKmssVectorData(long KmsId);
    }
}
