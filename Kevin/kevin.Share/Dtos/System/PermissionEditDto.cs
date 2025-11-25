using kevin.Permission.Permission;

namespace kevin.Domain.Share.Dtos.System
{
    public class PermissionEditDto
    {
        public string roleId { get; set; } = "";

        public List<string> permissionIds { get; set; } = new List<string>();

        public List<PermissionTypeDto> dtos { get; set; } = new List<PermissionTypeDto>();
    }

    public class PermissionTypeDto
    {

        /// <summary>
        /// 权限类型
        /// </summary>
        public string PermissionType
        {
            get; set;
        }
        public List<AreaPermissionDto> dtos { get; set; } = new List<AreaPermissionDto>(); 
    }
}
