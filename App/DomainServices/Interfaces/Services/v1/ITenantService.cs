using kevin.Domain.Entities;
using kevin.Domain.Share.Dtos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Interfaces.Services.v1
{
    public interface ITenantService
    {
        /// <summary>
        /// 设置无效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Inactive(Guid id);
        /// <summary>
        /// 设置有效租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Active(Guid id);
        /// <summary>
        /// 编辑租户
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<bool> EidtAsync(dtoTenant tenant);
        /// <summary>
        /// 新增租户
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<bool> CreateAsync(dtoTenant tenant);
       /// <summary>
       /// 删除租户
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 初始化租户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> InitializedData(dtoTenant tenant);
    }
}
