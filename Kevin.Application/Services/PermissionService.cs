using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Kevin;
using kevin.Domain.Share.Dtos.System;
using kevin.Ioc.IocAttrributes;
using kevin.Permission.Interfaces;
using kevin.Permission.Permission;
using kevin.Permission.Permisson;
using Microsoft.AspNetCore.Http;
using Web.Global.Exceptions;
namespace kevin.Application
{
    public class PermissionService : BaseService, IPermissionService
    {
        [IocProperty]
        public IKevinPermissionService kevinPermissionService { get; set; }

        public IRolePermissionRp rolePermissionRp { get; set; }
        public IPermissionRp permissionRp { get; set; }
        public IUserRp userRp { get; set; }
        public PermissionService(IHttpContextAccessor _httpContextAccessor, IUserRp _IUserRp, IPermissionRp _IPermissionRp, IRolePermissionRp _IRolePermissionRp, IKevinPermissionService _IKevinPermissionService) : base(_httpContextAccessor)
        {
            this.userRp = _IUserRp;
            this.permissionRp = _IPermissionRp;
            this.rolePermissionRp = _IRolePermissionRp;
            this.kevinPermissionService = _IKevinPermissionService;
        }

        /// <summary>
        /// 初始化权限
        /// </summary> 
        /// <returns></returns>
        public bool Reload()
        {
            var all = (new GlobalData()).AllModules;
            all.ForEach(r =>
            {
                r.Id = $"{r.Area}.{r.Module}.{r.Action}";
            });
            var areas = all.Select(x => x.Area).ToList();
            var allExist = permissionRp.Query().Where(r => r.IsManual == false && areas.Any(x => x == r.Area)).ToList();
            var allExistIds = allExist.Select(r => r.Id).ToList();

            var preAdd = all.Where(r => !allExistIds.Contains(r.Id)).ToList();
            var preDelete = allExist.Where(r => !all.Any(p => p.Id == r.Id));
            var preDeleteIds = preDelete.Select(r => r.Id).ToList();
            var preDeleteRoleP = rolePermissionRp.Query().Where(r => preDeleteIds.Contains(r.PermissionId)).ToList();
            permissionRp.AddRange(preAdd);
            permissionRp.RemoveRange(preDelete.ToArray());
            rolePermissionRp.RemoveRange(preDeleteRoleP.ToArray());
            return permissionRp.SaveChanges() > 0 && rolePermissionRp.SaveChanges() > 0;

        }

        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TPermission GetDetails(string Id)
        {
            var entity = permissionRp.Query().Where(t => t.Id == Id).FirstOrDefault();
            if (entity != null) return entity;
            else throw new UserFriendlyException("权限不存在");
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del(string id)
        {
            var entity = permissionRp.Query().Where(t => t.Id == id).FirstOrDefault();
            if (entity == null) return false;
            if (entity.IsManual.HasValue != true)
            {
                throw new UserFriendlyException("系统权限不能删除");
            }
            permissionRp.RemoveRange(entity);
            int res = permissionRp.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Edit(TPermission entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = $"{entity.Area}.{entity.Module}.{entity.Action}";
                entity.CreateTime = DateTime.Now;
                entity.IsManual = true;
                entity.CreateUserId = CurrentUser.UserId;
                permissionRp.Add(entity);
            }
            else
            {
                var data = permissionRp.Query().Where(t => t.Id == entity.Id).FirstOrDefault();
                if (data.IsManual.HasValue != true)
                {
                    throw new UserFriendlyException("系统权限不能操作");
                }
                data.UpdatedTime = DateTime.Now;
                data.UpdateUserId = CurrentUser.UserId;
                entity.MapTo(data);
            }
            int res = permissionRp.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        public List<TPermission> GetAllPermissions()
        {
            return permissionRp.Query().Where(x => x.IsDelete == false).ToList();
        }

        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        public List<string> GetAllPermissionIds()
        {
            return permissionRp.Query().Where(x => x.IsDelete == false).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        public List<AreaPermissionDto> GetAllAreaPermissions(Guid roleId)
        {
            var data = permissionRp.Query().Where(x => x.IsDelete == false).ToList();


            var list = new List<AreaPermissionDto>();
            var areas = data.Select(x => x.Area).Distinct().ToList();
            var reolePer = rolePermissionRp.Query().Where(x => x.IsDelete == false && x.RoleId == roleId).Select(x => x.PermissionId).ToList();

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

        /// <summary>
        /// 编辑角色对应权限
        /// </summary> 
        /// <returns></returns>
        public bool EditAllAreaPermissions(PermissionEditDto dto)
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
            var allP = rolePermissionRp.Query().Where(r => r.RoleId == roleId).ToList();
            var preAdd = rolepers.Where(r => !allP.Any(p => p.PermissionId == r.PermissionId));
            var preDelete = allP.Where(r => !rolepers.Any(p => p.PermissionId == r.PermissionId));
            rolePermissionRp.AddRange(preAdd);
            rolePermissionRp.RemoveRange(preDelete.ToArray());
            rolePermissionRp.SaveChanges();
            return true;
        }

        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        public List<string> GetUserPermissions()
        {
            var user = userRp.Query().Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
            if (user != null)
            {
                if (user.IsSuperAdmin)
                {
                    return kevinPermissionService.GetUserPermissions(CurrentUser.UserId.ToString()).Select(x => x.Key).ToList();
                }
            }
            return kevinPermissionService.GetUserPermissions(CurrentUser.UserId.ToString()).Where(x => x.Value == true).Select(x => x.Key).ToList();

        }
    }
}
