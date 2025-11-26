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
           new TTenant(TenantHelper.GetSettingsTenantId().ToTryInt32(),"admin",DateTime.Parse("2020-01-01 00:00:01")) { Id = 4514140354251222771}
        };
    }
}
