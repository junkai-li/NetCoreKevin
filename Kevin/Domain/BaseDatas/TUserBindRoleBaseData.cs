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
                    Id= 4514140314251221771,
                    UserId = item.Id,
                    RoleId = TRoleBaseData.Id,
                    TenantId = TenantHelper.GetSettingsTenantId().ToTryInt32(),
                    CreateUserId = item.Id,
                    CreateTime= DateTime.Parse("2020-01-01 00:00:01")
                });
            }
            return TUserBindRoles;
        }
    }
}
