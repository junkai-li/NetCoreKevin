using App.Domain.Events;
using App.Domain.Interfaces.Repositorie.v1;
using App.Domain.Interfaces.Services.v1;
using kevin.Domain.BaseDatas;
using kevin.Domain.Configuration;
using kevin.Domain.Entities;
using kevin.Domain.EventBus;
using kevin.Domain.Kevin;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace App.Application.Services.v1
{
    public class TenantService : BaseService, ITenantService
    {
        public ITenantRp tenantRp { get; set; }
        public IUserRp userRp { get; set; }
        public IRoleRp roleRp { get; set; }
        public TenantService(IHttpContextAccessor _httpContextAccessor, ITenantRp _tenantRp, IUserRp _userRp, IRoleRp _roleRp) : base(_httpContextAccessor)
        {
            tenantRp = _tenantRp;
            this.userRp = _userRp;
            this.roleRp = _roleRp;
        }

        public async Task<bool> Inactive(Guid id)
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
        public async Task<bool> Active(Guid id)
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

        public async Task<bool> EidtAsync(dtoTenant tenant)
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

        public async Task<bool> CreateAsync(dtoTenant tenant)
        {
            var tenantcode = tenantRp.Query().FirstOrDefault(t => t.Code == tenant.Code);
            if (tenantcode != default)
            {
                throw new UserFriendlyException(tenantcode.Code + "租户Code已存在");
            }
            var addtenant = new TTenant(tenant.Code, tenant.Name);
            addtenant.AddDomainEvent(new TTenantCreatedEvent(addtenant),EventBusEnums.Add);
            tenantRp.Add(addtenant);
            tenantRp.SaveChanges();
            return true; 
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
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
        public async Task<bool> InitializedData(dtoTenant tenant) 
        { 
            var role = new TRole();
            role.Id = Guid.NewGuid();
            role.Name = "管理员";
            role.Remarks = "初始化角色";
            role.CreateTime = DateTime.Now;
            role.TenantId=tenant.Code; 
            var user = new TUser();
            user.Id = Guid.NewGuid();
            user.Name = "admin";
            user.NickName = "admin";
            user.Phone = "admin";
            user.PassWord = "admin123";
            user.IsSuperAdmin = true;
            user.CreateTime = DateTime.Now;
            user.TenantId =tenant.Code;
            user.RoleId= role.Id;
            user.Email = "admin";
            roleRp.Add(role);
            userRp.Add(user); 
            return true;
        }
    }
}
