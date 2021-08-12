using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApiService.Permission;
using WebApiService.Permisson;
using WebApiService.Permisson.Attributes;
using WebService.Actions;

namespace AdminApi.Controllers
{
    [Route("api/[controller]")]
    [MyArea("系统管理", "System")]
    [ApiController]
    [ActionDescription("权限管理")]
    public class PermissionController : ControllerBase
    {
        /// <summary>
        /// 初始化权限
        /// </summary> 
        /// <returns></returns>
        [HttpGet("Reload")]
        [ActionDescription("初始化")]
        [SkipAuthority]
        public bool Reload()
        {
            var all = (new GlobalData()).AllModules;
            all.ForEach(r =>
            {
                r.Id = $"{r.Area}.{r.Module}.{r.Action}";
            });
            using (var Db = new dbContext())
            {
                var areas = all.Select(x => x.Area).ToList();
                var allExist = Db.Set<TPermission>().Where(r => r.IsManual == false && areas.Any(x => x == r.Area)).ToList();
                var allExistIds = allExist.Select(r => r.Id).ToList();

                var preAdd = all.Where(r => !allExistIds.Contains(r.Id)).ToList();
                var preDelete = allExist.Where(r => !all.Any(p => p.Id == r.Id));
                var preDeleteIds = preDelete.Select(r => r.Id).ToList();
                var preDeleteRoleP = Db.Set<TRolePermission>().Where(r => preDeleteIds.Contains(r.PermissionId)).ToList();
                Db.AddRange(preAdd);
                Db.RemoveRange(preDelete);
                Db.RemoveRange(preDeleteRoleP);
                return Db.SaveChanges() > 0;
            }

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
            using (var Db = new dbContext())
            {
                var entity = Db.Set<TPermission>().Where(t => t.Id == Id).FirstOrDefault();
                if (entity != null) return entity;
                else ResponseErrAction.ExceptionMessage("权限不存在"); return default;
            }
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
            using (var Db = new dbContext())
            {
                var entity = Db.Set<TPermission>().Where(t => t.Id == id).FirstOrDefault();
                if (entity == null) return false;
                if (entity.IsManual.HasValue != true)
                {
                    ResponseErrAction.ExceptionMessage("系统权限不能删除"); return default;
                }
                Db.RemoveRange(entity);
                int res = Db.SaveChanges();
                return res > 0;
            }
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
            using (var Db = new dbContext())
            {
                var userId = Guid.Parse(WebApiService.Libraries.Verify.JwtToken.GetClaims("userid"));
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = $"{entity.Area}.{entity.Module}.{entity.Action}";
                    entity.CreateTime = DateTime.Now;
                    entity.IsManual = true;
                    entity.CreateUserId = userId;
                    Db.Set<TPermission>().Add(entity);
                }
                else
                {
                    var data = Db.Set<TPermission>().Where(t => t.Id == entity.Id).FirstOrDefault();
                    if (data.IsManual.HasValue != true)
                    {
                        ResponseErrAction.ExceptionMessage("系统权限不能操作"); return default;
                    }
                    data.UpdatedTime = DateTime.Now;
                    data.UpdateUserId = userId;
                    entity.MapTo(data);
                }
                int res = Db.SaveChanges();
                return res > 0;
            }
        }



        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAllPermissions")]
        [ActionDescription("查看所有权限")]
        public List<TPermission> GetAllPermissions()
        {
            using (var Db = new dbContext())
            {
                return Db.TPermission.Where(x => x.IsDelete == false).ToList();
            }
        }
        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAllPermissionIds")]
        [SkipAuthority]
        public List<string> GetAllPermissionIds()
        {
            using (var Db = new dbContext())
            {
                return Db.TPermission.Where(x => x.IsDelete == false).Select(x => x.Id).ToList();
            }
        }

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        [SkipAuthority]
        [HttpGet("GetAllAreaPermissions")]
        public List<AreaPermissionDto> GetAllAreaPermissions([Required] Guid roleId)
        {
            using (var Db = new dbContext())
            {

                var data = Db.TPermission.Where(x => x.IsDelete == false).ToList();


                var list = new List<AreaPermissionDto>();
                var areas = data.Select(x => x.Area).Distinct().ToList();
                var reolePer = Db.TRolePermission.Where(x => x.IsDelete == false && x.RoleId == roleId).Select(x => x.PermissionId).ToList();

                foreach (var area in areas)
                {
                    var areadto = new AreaPermissionDto();
                    areadto.areaName = data.Where(x => x.Area == area).FirstOrDefault().AreaName;
                    areadto.area = area;
                    var modules = data.Where(x => x.Area == area).Select(x => x.Module).Distinct().ToList();
                    var modulesDtos = new List<ModulePermissionDto>();
                    foreach (var module in modules)
                    {
                        var moduledto = new ModulePermissionDto();
                        moduledto.ModuleName = data.Where(x => x.Area == area && x.Module == module).FirstOrDefault().ModuleName;
                        moduledto.Module = module;
                        var actionDtos = data.Where(x => x.Area == area && x.Module == module).OrderByDescending(x => x.Seq).Select(x => new ActionPermissionDto
                        {
                            Id = x.Id,
                            IsPermission = reolePer.Contains(x.Id),
                            ActionName = x.ActionName,
                            Action = x.Action,
                            FullName = x.FullName,
                            HttpMethod = x.HttpMethod,
                            IsManual = x.IsManual,
                            Seq = x.Seq,
                            Icon = x.Icon
                        }).ToList();
                        moduledto.actions = actionDtos;
                        modulesDtos.Add(moduledto);
                    }
                    areadto.modules = modulesDtos;
                    list.Add(areadto);
                }
                return list;
            }
        }

        /// <summary>
        /// 编辑角色对应权限
        /// </summary> 
        /// <returns></returns>
        [SkipAuthority]
        [HttpPost("EditAllAreaPermissions")]
        public bool EditAllAreaPermissions([Required] PermissionEditDto dto)
        {
            using (var Db = new dbContext())
            {
                var rolepers = new List<TRolePermission>();
                foreach (var perdto in dto.dtos)
                {
                    foreach (var module in perdto.modules)
                    {
                        foreach (var action in module.actions.Where(x => x.IsPermission == true).ToList())
                        {
                            var roleper = new TRolePermission();
                            roleper.PermissionId = action.Id;
                            roleper.RoleId = Guid.Parse(dto.roleId);
                            roleper.Id = Guid.NewGuid();
                            rolepers.Add(roleper);
                        }
                    }
                }
                var roleId = rolepers.FirstOrDefault().RoleId;
                var allP = Db.TRolePermission.Where(r => r.RoleId == roleId).ToList();
                var preAdd = rolepers.Where(r => !allP.Any(p => p.PermissionId == r.PermissionId));
                var preDelete = allP.Where(r => !rolepers.Any(p => p.PermissionId == r.PermissionId));
                Db.AddRange(preAdd);
                Db.RemoveRange(preDelete);
                Db.SaveChanges();
            } 
            return true;
        }


        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        [HttpGet("GetUserPermissions")]
        [SkipAuthority]
        public List<string> GetUserPermissions()
        {
            using (var db = new dbContext())
            {
                var userid = Guid.Parse(WebApiService.Libraries.Verify.JwtToken.GetClaims("userid"));
                var user = db.TUser.Where(x => x.IsDelete == false && x.Id == userid).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsSuperAdmin)
                    {
                        return PermissionsAction.GetUserPermissions(WebApiService.Libraries.Verify.JwtToken.GetClaims("userid")).Select(x => x.Key).ToList();
                    }
                }
                return PermissionsAction.GetUserPermissions(WebApiService.Libraries.Verify.JwtToken.GetClaims("userid")).Where(x => x.Value == true).Select(x => x.Key).ToList();
            } 
        }
    }
}
