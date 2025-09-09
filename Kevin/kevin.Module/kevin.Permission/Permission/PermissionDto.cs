using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Permission.Permisson
{
    public class PermissionDto
    {
        public   string? Id { get; set; }
        public   string? AreaName { get; set; }
        public int? Seq { get; set; }
        public   string? ModuleName { get; set; }
        public   string? ActionName { get; set; }
        public bool IsAccess { get; set; }

    }
}
