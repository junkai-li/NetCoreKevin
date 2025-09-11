using kevin.Permission.Permission;

namespace kevin.Domain.Share.Dtos.System
{
    public  class PermissionEditDto
    {
        public string roleId { get; set; } = "";

       public List<AreaPermissionDto> dtos { get; set; } = new List<AreaPermissionDto>();
    }
}
