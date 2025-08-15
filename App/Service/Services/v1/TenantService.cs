using App.Domain.Interfaces.Repositorie.v1;
using App.Domain.Interfaces.Services.v1;
using App.RepositorieRps.Repositories.v1;
using kevin.Domain.Entities;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Enums;
using Microsoft.AspNetCore.Http;
using Service.Services.v1._;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace App.Application.Services.v1
{
    public class TenantService : BaseService, ITenantService
    {
        public ITenantRp tenantRp { get; set; }
        public TenantService(IHttpContextAccessor _httpContextAccessor, ITenantRp _ITenantRp) : base(_httpContextAccessor)
        {
            tenantRp = _ITenantRp;
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
            var addtenant = new TTenant();
            addtenant.Name = tenant.Name;
            addtenant.Code = tenant.Code;
            addtenant.Id = Guid.NewGuid();
            addtenant.Status = TenantStatusEnums.Active;
            addtenant.IsDelete = false;
            addtenant.CreateTime= DateTime.Now;
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
    }
}
