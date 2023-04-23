using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq; 
using Web.Actions;
using WebApi.Controllers.Bases;
using Microsoft.AspNetCore.Authorization;
using Service.Services.v1._;
using kevin.Permission.Permisson.Attributes;
using kevin.Permission.Permisson;
using kevin.Permission.Permission;

namespace AdminApi.Controllers
{
    [Route("api/[controller]")]
    [MyArea("系统管理", "System")]
    [ActionDescription("权限管理")] 
    [Authorize]
    public class PermissionController : ApiControllerBase
    {
        private IPermissionService _permissionService { get; set; }

        public PermissionController(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        /// <summary>
        /// 初始化权限
        /// </summary> 
        /// <returns></returns>
        [HttpGet("Reload")]
        [ActionDescription("初始化")]
        [SkipAuthority]
        public bool Reload()
        {
            return _permissionService.Reload();
        }

        /// <summary>
        ///  获取
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("查看")]
        public dtoPageList<TPermission> GetPageData(dtoPageList<TPermission> input)
        {
            return default;
        }

        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetDetails")]
        [ActionDescription("查看详情")]
        public TPermission GetDetails(string Id)
        {
            return _permissionService.GetDetails(Id);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Del")]
        [ActionDescription("删除")]
        public bool Del(string id)
        {
            return _permissionService.Del(id);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        [ActionDescription("编辑")]
        public bool Edit(TPermission entity)
        {
            return _permissionService.Edit(entity);
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAllPermissions")]
        [ActionDescription("查看所有权限")]
        public List<TPermission> GetAllPermissions()
        {
            return _permissionService.GetAllPermissions();
        }
        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAllPermissionIds")]
        [SkipAuthority]
        public List<string> GetAllPermissionIds()
        {
            return _permissionService.GetAllPermissionIds();
        }

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        [SkipAuthority]
        [HttpGet("GetAllAreaPermissions")]
        public List<AreaPermissionDto> GetAllAreaPermissions([Required] Guid roleId)
        {
            return _permissionService.GetAllAreaPermissions(roleId);
        }

        /// <summary>
        /// 编辑角色对应权限
        /// </summary> 
        /// <returns></returns>
        [SkipAuthority]
        [HttpPost("EditAllAreaPermissions")]
        public bool EditAllAreaPermissions([Required] PermissionEditDto dto)
        {
            return _permissionService.EditAllAreaPermissions(dto);
        }


        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        [HttpGet("GetUserPermissions")]
        [SkipAuthority]
        public List<string> GetUserPermissions()
        {
            return _permissionService.GetUserPermissions();
        }
    }
}
