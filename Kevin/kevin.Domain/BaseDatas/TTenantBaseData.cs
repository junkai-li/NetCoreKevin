using Kevin.Common.App;
using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kevin.Domain.Entities;
using kevin.Domain.Share.Enums;

namespace kevin.Domain.BaseDatas
{
    public class TTenantBaseData
    {
        public static List<TTenant> TTenants { get; set; } = new List<TTenant>()
        {
           new TTenant { Id = Guid.NewGuid(), Name = "admin",Code=1000,CreateTime=DateTime.Now,Status=TenantStatusEnums.Active,IsDelete=false}
        };
    }
}
