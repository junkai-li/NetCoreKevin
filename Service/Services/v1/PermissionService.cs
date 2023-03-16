using Repository.Database;
using Service.Services.v1._;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Actions;
using Web.Global.Exceptions;
using Web.Permission;
using Web.Permisson;

namespace Service.Services.v1
{
    public class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
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
            var allExist = db.Set<TPermission>().Where(r => r.IsManual == false && areas.Any(x => x == r.Area)).ToList();
            var allExistIds = allExist.Select(r => r.Id).ToList();

            var preAdd = all.Where(r => !allExistIds.Contains(r.Id)).ToList();
            var preDelete = allExist.Where(r => !all.Any(p => p.Id == r.Id));
            var preDeleteIds = preDelete.Select(r => r.Id).ToList();
            var preDeleteRoleP = db.Set<TRolePermission>().Where(r => preDeleteIds.Contains(r.PermissionId)).ToList();
            db.AddRange(preAdd);
            db.RemoveRange(preDelete);
            db.RemoveRange(preDeleteRoleP);
            return db.SaveChanges() > 0;

        }

        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TPermission GetDetails(string Id)
        {
            var entity = db.Set<TPermission>().Where(t => t.Id == Id).FirstOrDefault();
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
            var entity = db.Set<TPermission>().Where(t => t.Id == id).FirstOrDefault();
            if (entity == null) return false;
            if (entity.IsManual.HasValue != true)
            {
                throw new UserFriendlyException("系统权限不能删除");
            }
            db.RemoveRange(entity);
            int res = db.SaveChanges();
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
                db.Set<TPermission>().Add(entity);
            }
            else
            {
                var data = db.Set<TPermission>().Where(t => t.Id == entity.Id).FirstOrDefault();
                if (data.IsManual.HasValue != true)
                {
                    throw new UserFriendlyException("系统权限不能操作");
                }
                data.UpdatedTime = DateTime.Now;
                data.UpdateUserId = CurrentUser.UserId;
                entity.MapTo(data);
            }
            int res = db.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        public List<TPermission> GetAllPermissions()
        {
            return db.TPermission.Where(x => x.IsDelete == false).ToList();
        }

        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        public List<string> GetAllPermissionIds()
        {
            return db.TPermission.Where(x => x.IsDelete == false).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        public List<AreaPermissionDto> GetAllAreaPermissions(Guid roleId)
        {
            var data = db.TPermission.Where(x => x.IsDelete == false).ToList();


            var list = new List<AreaPermissionDto>();
            var areas = data.Select(x => x.Area).Distinct().ToList();
            var reolePer = db.TRolePermission.Where(x => x.IsDelete == false && x.RoleId == roleId).Select(x => x.PermissionId).ToList();

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
            var allP = db.TRolePermission.Where(r => r.RoleId == roleId).ToList();
            var preAdd = rolepers.Where(r => !allP.Any(p => p.PermissionId == r.PermissionId));
            var preDelete = allP.Where(r => !rolepers.Any(p => p.PermissionId == r.PermissionId));
            db.AddRange(preAdd);
            db.RemoveRange(preDelete);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        public List<string> GetUserPermissions()
        {
            var user = db.TUser.Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
            if (user != null)
            {
                if (user.IsSuperAdmin)
                {
                    return PermissionsAction.GetUserPermissions(CurrentUser.UserId.ToString()).Select(x => x.Key).ToList();
                }
            }
            return PermissionsAction.GetUserPermissions(CurrentUser.UserId.ToString()).Where(x => x.Value == true).Select(x => x.Key).ToList();

        }
    }
}
