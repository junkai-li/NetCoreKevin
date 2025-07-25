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
        public static Guid Id=Guid.NewGuid();
        public static List<TRole> TRoles { get; set; } = new List<TRole>()
        {
           new TRole { Id = Id, Name = "admin",Remarks="admin", CreateTime = DateTime.Now,TenantId=TenantHelper.GetSettingsTenantId()}
        };
    }
}
