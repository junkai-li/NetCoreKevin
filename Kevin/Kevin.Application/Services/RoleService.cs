using Aop.Api.Domain;
using kevin.Domain.BaseDatas;
using kevin.Domain.Configuration;
using kevin.FileStorage;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace kevin.Application.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public IRoleRp RoleRp { get; set; }
        public RoleService(IHttpContextAccessor _httpContextAccessor, IRoleRp roleRp) : base(_httpContextAccessor)
        {
            RoleRp = roleRp;
        }

        /// <summary>
        /// 获取用户角色列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<dtoRole>> GetRolePage(dtoPageData<dtoRole> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = RoleRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey) || (t.Remarks ?? "").Contains(dtoPage.searchKey));
            }
            dtoPage.total = await data.CountAsync();
            dtoPage.data = await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name ?? "",
                Remarks = t.Remarks ?? "",
                CreateTime = t.CreateTime
            }).ToListAsync();

            return dtoPage;
        }
        public async Task<bool> AddEidt(dtoRole dtoRole)
        {
            if (dtoRole != default)
            {
                if (dtoRole.Id == default || dtoRole.Id == Guid.Empty)
                {
                    var add = new TRole
                    {
                        Id = Guid.NewGuid(),
                        Name = dtoRole.Name,
                        Remarks = dtoRole.Remarks,
                        CreateTime = DateTime.Now,
                        TenantId = CurrentUser.TenantId,
                        IsDelete = false
                    };
                    RoleRp.Add(add);
                    return RoleRp.SaveChanges() > 0;
                }
                else
                {
                    var edit = RoleRp.FirstOrDefault(t => t.Id == dtoRole.Id && t.IsDelete == false);
                    if (edit != default)
                    {
                        edit.Name = dtoRole.Name;
                        edit.Remarks = dtoRole.Remarks;
                        RoleRp.Update(edit);
                        return RoleRp.SaveChanges() > 0;
                    }
                    else
                    {
                        throw new UserFriendlyException("角色不存在");
                    }
                }
            }
            return false;
        }

        public async Task<bool> DeleteRole(Guid roleId)
        {
            if (TRoleBaseData.TRoles.Where(t => t.Id == roleId).FirstOrDefault() != default)
            {
                throw new UserFriendlyException("种子数据不能删除");
            }
            var delete = RoleRp.FirstOrDefault(t => t.Id == roleId && t.IsDelete == false);
            if (delete != default)
            {
                delete.IsDelete = true;
                delete.DeleteTime = DateTime.Now;
                RoleRp.Update(delete);
                return (RoleRp.SaveChanges()) > 0;
            }
            return false;
        }

        public async Task<List<dtoRole>> GetAllRoles()
        {
            return await RoleRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name ?? "",
                Remarks = t.Remarks ?? "",
                CreateTime = t.CreateTime
            }).ToListAsync();
        }

        public async Task<dtoRole> GetRoleById(Guid roleId)
        {
            return await RoleRp.Query().Where(t => t.Id == roleId && t.IsDelete == false).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name ?? "",
                Remarks = t.Remarks ?? "",
                CreateTime = t.CreateTime
            }).FirstAsync();
        }
    }
}
