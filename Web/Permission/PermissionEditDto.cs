using Web.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Web.Permisson
{
    public  class PermissionEditDto
    {
        public string roleId { get; set; }

       public List<AreaPermissionDto> dtos { get; set; }
    }
}
