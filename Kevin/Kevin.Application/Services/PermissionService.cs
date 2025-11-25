using Consul;
using Emgu.CV.Dnn;
using kevin.Permission.Interfaces;
using kevin.Permission.Permission;
using kevin.Permission.Permisson;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
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

        public static string[] permissionTypeDic = new string[] { "其他", "菜单权限", "功能权限", "数据权限", "接口权限" };
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
        public bool Reload(int TenantId = default)
        {
            var userId = default(Guid);
            if (TenantId == default)
            {
                TenantId = CurrentUser.TenantId;
                userId = CurrentUser.UserId;
            }
            var all = (new GlobalData()).AllModules;
            all.ForEach(r =>
            {
                r.Id = $"{TenantId}/{r.Area}/{r.Module}/{r.Action}";
                r.PermissionType = 4;
                r.FullName = $"{r.AreaName}/{r.ModuleName}/{r.ActionName}";
            });
            var areas = all.Select(x => x.Area).ToList();
            var allExist = permissionRp.Query().Where(r => r.IsManual == false && areas.Any(x => x == r.Area)).ToList();
            var allExistIds = allExist.Select(r => r.Id).ToList();

            var preAdd = all.Where(r => !allExistIds.Contains(r.Id)).ToList();
            var preDelete = allExist.Where(r => !all.Any(p => p.Id == r.Id));
            var preDeleteIds = preDelete.Select(r => r.Id).ToList();
            var preDeleteRoleP = rolePermissionRp.Query().Where(r => preDeleteIds.Contains(r.PermissionId)).ToList();
            permissionRp.AddRange(preAdd.Select(t => new TPermission
            {
                CreateTime = DateTime.Now,
                Id = t.Id,
                TenantId = TenantId,
                CreateUserId = userId,
                Area = t.Area,
                AreaName = t.AreaName,
                Module = t.Module,
                ModuleName = t.ModuleName,
                PermissionType = t.PermissionType,
                Action = t.Action,
                ActionName = t.ActionName,
                FullName = t.FullName,
                HttpMethod = t.HttpMethod,
                IsManual = false,
                Seq = t.Seq,
                Icon = t.Icon,
            }));
            permissionRp.RemoveRange(preDelete.ToArray());
            rolePermissionRp.RemoveRange(preDeleteRoleP.ToArray());
            return permissionRp.SaveChanges() > 0 && rolePermissionRp.SaveChanges() > 0;

        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<dtoPageData<PermissionDto>> GetPageData(dtoPageData<PermissionDto> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = permissionRp.Query().Where(t => t.IsDelete == false);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.FullName ?? "").Contains(dtoPage.searchKey)
                    || (t.ActionName ?? "").Contains(dtoPage.searchKey)
                    || (t.AreaName ?? "").Contains(dtoPage.searchKey)
                     || (t.ModuleName ?? "").Contains(dtoPage.searchKey)
                    );
            }
            dtoPage.total = await data.CountAsync();
            dtoPage.data = await data.OrderByDescending(x => x.Area).Skip(skip).Take(dtoPage.pageSize).Select(t => new PermissionDto
            {
                Id = t.Id,
                CreateTime = t.CreateTime,
                CreateUserId = t.CreateUserId,
                Action = t.Action,
                ActionName = t.ActionName,
                Area = t.Area,
                AreaName = t.AreaName,
                FullName = t.FullName,
                HttpMethod = t.HttpMethod,
                Icon = t.Icon,
                IsManual = t.IsManual,
                Module = t.Module,
                ModuleName = t.ModuleName,
                Seq = t.Seq,
                TenantId = t.TenantId,
                UpdatedTime = t.UpdatedTime,
                UpdateUserId = t.UpdateUserId,
                PermissionType = t.PermissionType
            }).ToListAsync();
            return dtoPage;
        }


        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PermissionDto GetDetails(string Id)
        {
            var entity = permissionRp.Query().Where(t => t.Id == Id).FirstOrDefault();
            if (entity != null) return new PermissionDto
            {
                Id = entity.Id,
                CreateTime = entity.CreateTime,
                CreateUserId = entity.CreateUserId,
                Action = entity.Action,
                ActionName = entity.ActionName,
                Area = entity.Area,
                AreaName = entity.AreaName,
                FullName = entity.FullName,
                HttpMethod = entity.HttpMethod,
                Icon = entity.Icon,
                IsManual = entity.IsManual,
                Module = entity.Module,
                ModuleName = entity.ModuleName,
                Seq = entity.Seq,
                TenantId = entity.TenantId,
                UpdatedTime = entity.UpdatedTime,
                UpdateUserId = entity.UpdateUserId,
                PermissionType = entity.PermissionType
            };
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
            if (entity.IsManual != true)
            {
                throw new UserFriendlyException("系统权限不能删除");
            }
            permissionRp.Remove(entity);
            int res = permissionRp.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddEdit(PermissionDto parm)
        {
            if (string.IsNullOrEmpty(parm.Id))
            {
                //验证是否存在
                var repeatData = permissionRp.Query().Where(t => t.Area == parm.Area && t.Module == parm.Module && t.Action == parm.Action).FirstOrDefault();
                if (repeatData != default)
                {
                    throw new UserFriendlyException($"相同area中相同module的action不允许重复");
                }
                var adddata = new TPermission();
                parm.MapTo<PermissionDto, TPermission>(adddata);
                adddata.TenantId = CurrentUser.TenantId;
                adddata.Id = $"{CurrentUser.TenantId}/{parm.Area}/{parm.Module}/{parm.Action}";
                adddata.CreateTime = DateTime.Now;
                adddata.IsManual = true;
                adddata.CreateUserId = CurrentUser.UserId;
                permissionRp.Add(adddata);
            }
            else
            {
                var data = permissionRp.Query().Where(t => t.Id == parm.Id).FirstOrDefault();
                if (data != default)
                {
                    if (data.IsManual != true)
                    {
                        throw new UserFriendlyException("非手动添加禁止修改");
                    }
                    //验证是否重复
                    var repeatData = permissionRp.Query().Where(t => t.Id != parm.Id && t.Area == parm.Area && t.Module == parm.Module && t.Action == parm.Action).FirstOrDefault();
                    if (repeatData != default)
                    {
                        throw new UserFriendlyException($"相同area中相同module的action不允许重复");
                    }
                    parm.MapTo(data);
                    data.UpdatedTime = DateTime.Now;
                    data.UpdateUserId = CurrentUser.UserId;
                    data.IsManual = true;
                    data.TenantId = CurrentUser.TenantId;
                }
            }
            int res = permissionRp.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        public List<PermissionDto> GetAllPermissions()
        {
            return permissionRp.Query().Where(x => x.IsDelete == false).Select(t => new PermissionDto
            {
                Id = t.Id,
                CreateTime = t.CreateTime,
                CreateUserId = t.CreateUserId,
                Action = t.Action,
                ActionName = t.ActionName,
                Area = t.Area,
                AreaName = t.AreaName,
                FullName = t.FullName,
                HttpMethod = t.HttpMethod,
                Icon = t.Icon,
                IsManual = t.IsManual,
                Module = t.Module,
                ModuleName = t.ModuleName,
                Seq = t.Seq,
                TenantId = t.TenantId,
                UpdatedTime = t.UpdatedTime,
                UpdateUserId = t.UpdateUserId,
                PermissionType = t.PermissionType
            }).ToList();
        }

        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        public List<string> GetAllPermissionIds()
        {
            return permissionRp.Query().Where(x => x.IsDelete == false).Select(x => (x.Id ?? "")).ToList();
        }

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        public PermissionEditDto GetAllAreaPermissions(Guid roleId)
        {
            var data = permissionRp.Query().Where(x => x.IsDelete == false).ToList();
            var list = new PermissionEditDto();
            var permissionTypes = data.Select(x => x.PermissionType).Distinct().ToList();
            var reolePer = rolePermissionRp.Query().Where(x => x.IsDelete == false && x.RoleId == roleId).Select(x => x.PermissionId).ToList();
            foreach (var permissionType in permissionTypes)
            {
                var permissionTypeData = data.Where(x => x.PermissionType == permissionType).ToList();
                var areas = data.Where(x => x.PermissionType == permissionType).Select(x => x.Area).Distinct().ToList();
                var permissionDto = new PermissionTypeDto();
                permissionDto.PermissionType = permissionTypeDic[permissionType];
                foreach (var area in areas)
                {
                    if (!string.IsNullOrEmpty(area))
                    {
                        var areadto = new AreaPermissionDto();
                        var areaitem = permissionTypeData.Where(x => x.Area == area).FirstOrDefault();
                        if (areaitem != default)
                        {
                            areadto.areaName = areaitem.AreaName ?? "";
                        }
                        areadto.area = area;
                        var modules = permissionTypeData.Where(x => x.Area == area).Select(x => x.Module).Distinct().ToList();
                        var modulesDtos = new List<ModulePermissionDto>();
                        foreach (var module in modules)
                        {
                            if (!string.IsNullOrEmpty(module))
                            {
                                var moduledto = new ModulePermissionDto();
                                var permission = permissionTypeData.Where(x => x.Area == area && x.Module == module).FirstOrDefault();
                                if (permission != default)
                                {
                                    moduledto.ModuleName = permission.ModuleName ?? "";
                                }
                                moduledto.Module = module;
                                var actionDtos = permissionTypeData.Where(x => x.Area == area && x.Module == module).OrderByDescending(x => x.Seq).Select(x => new ActionPermissionDto
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

                        }
                        areadto.modules = modulesDtos;
                        permissionDto.dtos.Add(areadto);
                    }
                }
                list.dtos.Add(permissionDto);
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
            foreach (var item in dto.permissionIds)
            {
                var roleper = new TRolePermission();
                roleper.PermissionId = item;
                roleper.RoleId = Guid.Parse(dto.roleId);
                roleper.Id = Guid.NewGuid();
                roleper.TenantId = CurrentUser.TenantId;
                roleper.CreateTime = DateTime.Now;
                roleper.CreateUserId = CurrentUser.UserId;
                rolepers.Add(roleper);
            }
            var roleper2 = rolepers.FirstOrDefault();
            if (roleper2 != default)
            {
                var roleId = roleper2.RoleId;
                var allP = rolePermissionRp.Query().Where(r => r.IsDelete == false && r.RoleId == roleId).ToList();
                var preAdd = rolepers.Where(r => !allP.Any(p => p.PermissionId == r.PermissionId)).ToList();
                var preDelete = allP.Where(r => !rolepers.Any(p => p.PermissionId == r.PermissionId)).ToList();
                if (preAdd.Count > 0)
                {
                    rolePermissionRp.AddRange(preAdd);
                }
                if (preDelete.Count > 0)
                {
                    foreach (var item in preDelete)
                    {
                        item.IsDelete = true;
                        item.DeleteTime = DateTime.Now;
                        item.DeleteUserId = CurrentUser.UserId;
                    } 
                }
                rolePermissionRp.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        public List<string> GetUserPermissions()
        {
            var user = userRp.Query().Where(x => x.IsDelete == false).FirstOrDefault();
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
