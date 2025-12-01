using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Interfaces;
using kevin.Share.Dtos;

namespace kevin.Domain.Interfaces.IServices
{
    public interface ITenantService : IService
    {

        /// <summary>
        /// 租户列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        Task<dtoPageData<dtoTenant>> GetPageData(dtoPagePar<string> par);
        /// <summary>
        /// 设置无效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Inactive(long id, CancellationToken cancellationToken);
        /// <summary>
        /// 设置有效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Active(long id, CancellationToken cancellationToken);
        /// <summary>
        /// 编辑租户
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<bool> EidtAsync(dtoTenant tenant, CancellationToken cancellationToken);
        /// <summary>
        /// 新增租户
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<bool> CreateAsync(dtoTenant tenant, CancellationToken cancellationToken);
        /// <summary>
        /// 删除租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);

        /// <summary>
        /// 初始化租户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> InitializedData(dtoTenant tenant, CancellationToken cancellationToken);
    }
}
