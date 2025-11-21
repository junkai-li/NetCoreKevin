using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.WebApi.Controllers.v1
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("系统管理", "System")] 
    [MyModule("角色管理", "Role")]
    public class RoleController : ControllerBase
    { 
        private IRoleService _roleService { get; set; }

        public RoleController(IRoleService roleService)
        {
            this._roleService = roleService;
        }

        [HttpPost("GetRolePage")]
        [ActionDescription("获取分页角色数据")]
        [HttpLog("角色管理", "获取分页角色数据")]
        public async Task<dtoPageData<dtoRole>> GetRolePage([FromBody] dtoPageData<dtoRole> dtoPage)
        {
            var result = await _roleService.GetRolePage(dtoPage);
            return result;
        }

        [HttpPost("AddEidt")]
        [ActionDescription("新增或编辑角色数据")]
        [HttpLog("角色管理", "新增或编辑角色数据")]
        public async Task<bool> AddEidt([FromBody] dtoRole dtoRole)
        {
            var result = await _roleService.AddEidt(dtoRole);
            return result;
        }

        [ActionDescription("删除角色数据")]
        [HttpLog("角色管理", "删除角色数据")]
        [HttpDelete("DeleteRole")]
        public async Task<bool> DeleteRole([FromQuery]Guid roleId)
        {
            var result = await _roleService.DeleteRole(roleId);
            return result;
        }

        [ActionDescription("获取指定角色数据")]
        [HttpLog("角色管理", "获取指定角色数据")]
        [HttpGet("GetRoleById")]
        public async Task<dtoRole> GetRoleById(Guid roleId)
        {
            var result = await _roleService.GetRoleById(roleId);
            return result;
        }

        [ActionDescription("获取所有角色数据")]
        [HttpLog("角色管理", "获取所有角色数据")]
        [HttpGet("GetAllRoles")]
        public async Task<List<dtoRole>> GetAllRoles()
        {
            var result = await _roleService.GetAllRoles();
            return result;
        }
    }
}
