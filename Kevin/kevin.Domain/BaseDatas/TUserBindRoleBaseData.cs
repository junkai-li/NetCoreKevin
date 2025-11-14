using kevin.Domain.Configuration;
using kevin.Domain.Entities;
using Kevin.Common.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.BaseDatas
{ 
    public class TUserBindRoleBaseData
    { 
        public static List<TUserBindRole> GetUserBindRoles()
        {
            var TUserBindRoles = new List<TUserBindRole>();
            foreach (var item in TUserBaseData.TUsers)
            {
                TUserBindRoles.Add(new TUserBindRole
                {
                    Id= Guid.Parse("eef5525d-5d64-46ad-8d64-72fb3ad9728f"),
                    UserId = item.Id,
                    RoleId = TRoleBaseData.Id,
                    TenantId = TenantHelper.GetSettingsTenantId().ToTryInt32(),
                    CreateUserId = item.Id
                });
            }
            return TUserBindRoles;
        }
    }
}
