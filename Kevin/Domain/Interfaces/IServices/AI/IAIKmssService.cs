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
        Task<bool> ProcessKmssVectorData(long KmsId = default);

        /// <summary>
        /// 知识库文件碎片分页预览（只读实时查询向量库）
        /// </summary>
        /// <param name="kmsId">知识库Id</param>
        /// <param name="sourceFile">来源文件名（导入成功后回写的 ContentName）</param>
        /// <param name="pageNum">页码，从 1 开始</param>
        /// <param name="pageSize">每页条数（1..100）</param>
        /// <returns>碎片总数与当前页碎片列表；Qdrant 异常/无碎片时返回空结果</returns>
        Task<KmsChunksDto> GetKmsChunks(long kmsId, string sourceFile, int pageNum = 1, int pageSize = 10);
    }
}
