using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.System;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace App.WebApi.Controllers
{
    /// <summary>
    /// 租户管理
    /// </summary>
    [ApiVersionNeutral] 
    [Route("api/[controller]")] 
    [MyArea("租户管理", "TenantSystem")] 
    [MyModule("租户管理", "Tenant")] 
    [Authorize]
    [ApiController]
    public class TenantController : ApiControllerBase
    {
        [IocProperty]
        public ITenantService _ITenantService { get; set; }

        /// <summary>
        /// 设置无效租户
        /// </summary>
        /// <param data="keyValue">id租户</param>
        /// <returns></returns>
        [HttpPost("Inactive")]
        [ActionDescription("设置无效")]
        public async Task<bool> Inactive([FromBody] dtoId<string> data, CancellationToken cancellationToken)
        {
            return await _ITenantService.Inactive(Guid.Parse(data.Id), cancellationToken);
        }
        /// <summary>
        /// 设置有效租户
        /// </summary>
        /// <param data="keyValue">id租户</param>
        /// <returns></returns>
        [HttpPost("Active")]
        [ActionDescription("设置有效租户")]
        public async Task<bool> Active([FromBody] dtoId<string> data, CancellationToken cancellationToken)
        {
            return await _ITenantService.Active(Guid.Parse(data.Id), cancellationToken);
        }
        /// <summary>
        /// 新增租户
        /// </summary>
        /// <param data="dtoTenant">租户信息</param>
        /// <returns></returns>
        [HttpPost("CreateAsync")]
        [ActionDescription("新增租户")]
        public async Task<bool> CreateAsync([FromBody] dtoTenant data, CancellationToken cancellationToken)
        {
            return await _ITenantService.CreateAsync(data, cancellationToken);
        }
        /// <summary>
        /// 编辑租户
        /// </summary>
        /// <param data="dtoTenant">租户信息</param>
        /// <returns></returns>
        [HttpPost("EidtAsync")]
        [ActionDescription("编辑租户")]
        public async Task<bool> EidtAsync([FromBody] dtoTenant data, CancellationToken cancellationToken)
        {
            return await _ITenantService.EidtAsync(data, cancellationToken);
        }
        /// <summary>
        /// 删除租户
        /// </summary>
        /// <param data="keyValue">id租户</param>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
        [ActionDescription("删除租户")]
        public async Task<bool> DeleteAsync([FromBody] dtoId<string> data, CancellationToken cancellationToken)
        {
            return await _ITenantService.DeleteAsync(Guid.Parse(data.Id), cancellationToken);
        }
    }
}
