using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.System;
using kevin.Share.Dtos;

namespace App.Domain.Interfaces.Services.v1
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IAppDemoService : IBaseService
    {

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAppDemo>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAppDemo data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);

    }
}
