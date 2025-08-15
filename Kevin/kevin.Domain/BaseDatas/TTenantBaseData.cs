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
           new TTenant(1000,"admin") { Id = Guid.Parse("1b4f94ac-b697-4cbe-9626-6cd2de627538")}
        };
    }
}
