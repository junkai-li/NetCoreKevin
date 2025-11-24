using Kevin.Common.App;
using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.BaseDatas
{
    public class TRoleBaseData
    {
        public static Guid Id= Guid.Parse("c23301b7-f9e0-464c-b76d-1f0a5a557548");
        public static List<TRole> TRoles { get; set; } = new List<TRole>()
        {
           new TRole { Id = Id, Name = "admin",Remarks="admin", CreateTime = DateTime.Parse("2020-01-01 00:00:01"),TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32()}
        };
    }
}
