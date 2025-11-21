using Aop.Api.Domain;
using kevin.Application;
using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using kevin.Domain.EventBus;
using kevin.Domain.Events;
using kevin.Domain.Share.Enums;
using Microsoft.AspNetCore.Http;
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

        public async Task<bool> Inactive(Guid id, CancellationToken cancellationToken)
        {
            var tenant = tenantRp.Query().FirstOrDefault(t => t.Id == id);
            if (tenant != default)
            {
                tenant.Status = TenantStatusEnums.Inactive;
                tenant.UpdateTime = DateTime.Now;
                await tenantRp.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }
        public async Task<bool> Active(Guid id, CancellationToken cancellationToken)
        {
            var tenant = tenantRp.Query().FirstOrDefault(t => t.Id == id);
            if (tenant != default)
            {
                tenant.Status = TenantStatusEnums.Active;
                tenant.UpdateTime = DateTime.Now;
                await tenantRp.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }

        public async Task<bool> EidtAsync(dtoTenant tenant, CancellationToken cancellationToken)
        {
            var tenantcode = tenantRp.Query().FirstOrDefault(t => t.Id != Guid.Parse(tenant.Id) && t.Code == tenant.Code);
            if (tenantcode != default)
            {
                throw new UserFriendlyException(tenantcode.Code + "租户Code已存在");
            }
            var tenantdata = tenantRp.Query().FirstOrDefault(t => t.Id == Guid.Parse(tenant.Id));
            if (tenantdata != default)
            {
                tenantdata.Name = tenant.Name;
                tenantdata.Code = tenant.Code;
                tenantdata.UpdateTime = DateTime.Now;
                await tenantRp.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new UserFriendlyException("租户不存在");
            }
        }

        public Task<bool> CreateAsync(dtoTenant tenant, CancellationToken cancellationToken)
        {
            var tenantcode = tenantRp.Query().FirstOrDefault(t => t.Code == tenant.Code);
            if (tenantcode != default)
            {
                throw new UserFriendlyException(tenantcode.Code + "租户Code已存在");
            }
            var addtenant = new TTenant(tenant.Code, tenant.Name);
            addtenant.AddDomainEvent(new TTenantCreatedEvent(addtenant), EventBusEnums.Add);
            tenantRp.Add(addtenant);
            tenantRp.SaveChanges();
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (TTenantBaseData.TTenants.Where(t => t.Id == id).FirstOrDefault() != default)
            {
                throw new UserFriendlyException("种子数据不能删除");
            }
            var tenant = tenantRp.Query().FirstOrDefault(t => t.Id == id);
            if (tenant != default)
            {
                tenant.IsDelete = true;
                tenant.DeleteTime = DateTime.Now;
                await tenantRp.SaveChangesAsync();
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
        public   Task<bool> InitializedData(dtoTenant tenant, CancellationToken cancellationToken)
        {
            using var db = new KevinDbContext();
            var role = new TRole();
            role.Id = Guid.NewGuid();
            role.Name = "管理员";
            role.Remarks = "初始化角色";
            role.CreateTime = DateTime.Now;
            role.TenantId = tenant.Code;
            var user = new TUser();
            user.Id = Guid.NewGuid();
            user.Name = "admin";
            user.NickName = "admin";
            user.Phone = "admin";
            user.ChangePassword("admin123");
            user.IsSuperAdmin = true;
            user.CreateTime = DateTime.Now;
            user.TenantId = tenant.Code;
            user.Email = "admin";
            user.Status = true;
            var userbindrole = new TUserBindRole();
            userbindrole.Id = Guid.NewGuid();
            userbindrole.RoleId = role.Id;
            userbindrole.UserId = user.Id;
            userbindrole.CreateTime = DateTime.Now;
            userbindrole.TenantId = tenant.Code;
            db.Set<TUser>().Add(user);
            db.Set<TRole>().Add(role);
            db.SaveChanges();
            db.Set<TUserBindRole>().Add(userbindrole);
            db.SaveChanges();  
            return Task.FromResult(true);
        }
    }
}
