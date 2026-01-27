using kevin.Domain.BaseDatas;
using kevin.Domain.Configuration;
using kevin.Domain.Entities;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.EventBus;
using kevin.Domain.Events;
using kevin.Domain.Interfaces.IRepositories.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Enums;
using kevin.Share.Dtos;
using Repository.Database;
using Web.Global.Exceptions;

namespace kevin.Application
{
    public class TenantService : BaseService, ITenantService
    {
        public ITenantRp tenantRp { get; set; }
        public IUserRp userRp { get; set; }
        public IRoleRp roleRp { get; set; }

        public IUserBindRoleRp userBindRoleRp { get; set; }

        public IPermissionService permissionService { get; set; }
        public TenantService(IHttpContextAccessor _httpContextAccessor, ITenantRp _tenantRp, IUserRp _userRp, IRoleRp _roleRp, IUserBindRoleRp _userBindRoleRp, IPermissionService permissionService) : base(_httpContextAccessor)
        {
            tenantRp = _tenantRp;
            this.userRp = _userRp;
            this.roleRp = _roleRp;
            this.userBindRoleRp = _userBindRoleRp;
            this.permissionService = permissionService;
        }
        public async Task<dtoPageData<dtoTenant>> GetPageData(dtoPagePar<string> par)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            } 
            var dataPage = new dtoPageData<dtoTenant>();
            int skip = par.GetSkip();
            var data = tenantRp.Query(false).Where(t => t.IsDelete == false);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(par.searchKey));
            }
            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).ToListAsync();
            dataPage.data = dbdata.MapToList<TTenant, dtoTenant>();
            return dataPage;
        }
        public async Task<bool> Inactive(long id, CancellationToken cancellationToken)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            } 
            var tenant = await tenantRp.Query(false).FirstOrDefaultAsync(t => t.Id == id);
            if (tenant != default)
            {
                tenant.Status = TenantStatusEnums.Inactive;
                tenant.UpdateTime = DateTime.Now;
                 tenantRp.SaveChangesWithSaveLog();
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }
        public async Task<bool> Active(long id, CancellationToken cancellationToken)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            } 
            var tenant = await tenantRp.Query(false).FirstOrDefaultAsync(t => t.Id == id);
            if (tenant != default)
            {
                tenant.Status = TenantStatusEnums.Active;
                tenant.UpdateTime = DateTime.Now;
                tenantRp.SaveChangesWithSaveLog(); 
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }

        public async Task<bool> EidtAsync(dtoTenant tenant, CancellationToken cancellationToken)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            } 
            var tenantcode = await tenantRp.Query(false).FirstOrDefaultAsync(t => t.Id != tenant.Id.ToTryInt64() && t.Code == tenant.Code);
            if (tenantcode != default)
            {
                throw new UserFriendlyException(tenantcode.Code + "租户Code已存在");
            }
            var tenantdata = tenantRp.Query(false).FirstOrDefault(t => t.Id == tenant.Id.ToTryInt64());
            if (tenantdata != default)
            {
                tenantdata.Name = tenant.Name;
                tenantdata.Code = tenant.Code;
                tenantdata.UpdateTime = DateTime.Now;
                tenantRp.SaveChangesWithSaveLog();
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }

        public async Task<bool> CreateAsync(dtoTenant tenant, CancellationToken cancellationToken)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            } 
            var tenantcode = await tenantRp.Query(false).FirstOrDefaultAsync(t => t.Code == tenant.Code);
            if (tenantcode != default)
            {
                throw new UserFriendlyException(tenantcode.Code + "租户Code已存在");
            }
            var addtenant = new TTenant(tenant.Code, tenant.Name, DateTime.Now);
            addtenant.AddDomainEvent(new TTenantCreatedEvent(addtenant), EventBusEnums.Add);
            tenantRp.Add(addtenant);
            tenantRp.SaveChangesWithSaveLog();
            return  true;
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            }
            if (TTenantBaseData.TTenants.Where(t => t.Id == id).FirstOrDefault() != default)
            {
                throw new UserFriendlyException("种子数据不能删除");
            } 
            var tenant = await tenantRp.Query(false).FirstOrDefaultAsync(t => t.Id == id);
            if (tenant != default)
            {
                tenant.IsDelete = true;
                tenant.DeleteTime = DateTime.Now; 
                tenantRp.SaveChangesWithSaveLog(); 
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }


        /// <summary>
        /// 初始化租户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> InitializedData(dtoTenant tenant, CancellationToken cancellationToken)
        {
            using var db = new KevinDbContext();
            #region 初始化角色

            var addroles = new List<TRole>();
            foreach (var role in TRoleBaseData.TRoles)
            {
                role.Id = SnowflakeIdService.GetNextId();
                role.Name = "管理员";
                role.Remarks = "初始化角色";
                role.CreateTime = DateTime.Now;
                role.TenantId = tenant.Code;
                addroles.Add(role);
            }

            #endregion

            #region 初始化用户

            var addusers = new List<TUser>();
            foreach (var user in TUserBaseData.TUsers)
            {
                user.Id = SnowflakeIdService.GetNextId();
                user.Name = "admin";
                user.NickName = "admin";
                user.Phone = "admin";
                user.ChangePassword("admin123");
                user.IsSuperAdmin = true;
                user.CreateTime = DateTime.Now;
                user.TenantId = tenant.Code;
                user.Email = "admin";
                user.Status = true;
                addusers.Add(user);
            }

            #endregion

            #region 初始化用户角色

            var addUserBindRoles = new List<TUserBindRole>();
            foreach (var user in addusers)
            {
                var userbindrole = new TUserBindRole();
                userbindrole.Id = SnowflakeIdService.GetNextId();
                userbindrole.RoleId = addroles[0].Id;
                userbindrole.UserId = user.Id;
                userbindrole.CreateTime = DateTime.Now;
                userbindrole.TenantId = tenant.Code;
                userbindrole.CreateUserId= user.Id;
                addUserBindRoles.Add(userbindrole);
            }

            #endregion

            #region 初始化数据字典

            var addDicts = new List<TDictionary>();
            foreach (var item in TDictionaryBaseDatas.Data)
            {
                item.Id = SnowflakeIdService.GetNextId();
                item.CreateTime = DateTime.Now;
                item.TenantId = tenant.Code;
                item.CreateUserId = addusers[0].Id;
                addDicts.Add(item);
            }
            #endregion

            #region 初始化部门和岗位

            var addDepartments = new List<TDepartment>();
            foreach (var item in TDepartmentBaseDatas.Data)
            {
                item.Id = SnowflakeIdService.GetNextId();
                item.CreateTime = DateTime.Now;
                item.TenantId = tenant.Code;
                item.CreateUserId = addusers[0].Id;
                addDepartments.Add(item);
            }

            var addPositions = new List<TPosition>();
            foreach (var item in TPositionBaseDatas.Data)
            {
                item.Id = SnowflakeIdService.GetNextId();
                item.CreateTime = DateTime.Now;
                item.TenantId = tenant.Code;
                item.CreateUserId = addusers[0].Id;
                addPositions.Add(item);
            }

            #endregion

            #region 初始用户信息

            var addUserInfos = new List<TUserInfo>();
            foreach (var item in TUserInfoBaseDatas.GetData())
            {
                item.Id = SnowflakeIdService.GetNextId();
                item.CreateTime = DateTime.Now;
                item.TenantId = tenant.Code;
                item.CreateUserId = addusers[0].Id;
                item.DepartmentId = addDepartments[0].Id;
                item.UserId = addusers[0].Id;
                addUserInfos.Add(item);
            }

            #endregion

            db.Set<TUser>().AddRange(addusers);
            db.Set<TRole>().AddRange(addroles);
            db.SaveChanges();
            db.Set<TUserBindRole>().AddRange(addUserBindRoles);
            db.Set<TDictionary>().AddRange(addDicts);
            db.SaveChanges();
            db.Set<TDepartment>().AddRange(addDepartments);
            db.Set<TPosition>().AddRange(addPositions);
            db.SaveChanges();
            db.Set<TUserInfo>().AddRange(addUserInfos);
            db.SaveChanges();
            return Task.FromResult(true);
        }
    }
}
