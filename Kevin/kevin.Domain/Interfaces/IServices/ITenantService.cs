using kevin.Domain.Share.Dtos.System;

namespace kevin.Domain.Interfaces.IServices
{
    public interface ITenantService
    {
        /// <summary>
        /// 设置无效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Inactive(Guid id, CancellationToken cancellationToken);
        /// <summary>
        /// 设置有效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Active(Guid id, CancellationToken cancellationToken);
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
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// 初始化租户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> InitializedData(dtoTenant tenant, CancellationToken cancellationToken);
    }
}
