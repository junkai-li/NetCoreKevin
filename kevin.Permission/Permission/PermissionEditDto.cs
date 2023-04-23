using kevin.Permission.Permission;

namespace kevin.Permission.Permisson
{
    public  class PermissionEditDto
    {
        public string roleId { get; set; }

       public List<AreaPermissionDto> dtos { get; set; }
    }
}
