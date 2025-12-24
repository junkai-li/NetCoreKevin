using kevin.Domain.Share.Dtos.System;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IDictionaryService : IBaseService
    {   /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <returns></returns>
        Task<List<string?>> GetTypeList();

        /// <summary>
        /// 获取变动日志分页数据
        /// </summary>
        /// <param name="dtoPage"></param>
        /// <returns></returns>
        Task<dtoPageData<DictionaryDto>> GetPageData(dtoPagePar<string> dtoPage);
        /// <summary>
        /// 获取类型下的字典列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<DictionaryDto>> GetTypeWhereList(string type);
        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        Task<bool> AddEdit(DictionaryDto dic);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
