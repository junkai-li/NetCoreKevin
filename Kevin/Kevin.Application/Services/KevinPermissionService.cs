using kevin.Domain.Entities;
using kevin.Permission.Interfaces;
using Repository.Database;

namespace kevin.Application
{
    public class KevinPermissionService : BaseService, IKevinPermissionService
    {
        public KevinPermissionService(IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
        { 
        }


        public List<string> GetAllPermissionIds()
        {
            using (var db = new KevinDbContext())
            {
                return db.Set<TPermission>().Where(x => x.IsDelete == false).Select(x => (x.Id ?? "")).ToList(); 
            }
        }
        public List<long> GetUserRoleIds(string userId)
        {
            using var db = new KevinDbContext();
            var userBindRoles = db.Set<TUserBindRole>().Where(x => x.IsDelete == false && x.UserId ==userId.ToTryInt64()).ToList();
            if (userBindRoles != default && userBindRoles.Count > 0)
            {
                return userBindRoles.Select(x => x.RoleId).ToList();
            }
            else
            {
                // Handle the case where the user is not found or is deleted.
                // You might want to return an empty list or some other appropriate response.
                return new List<long>();
            }
        }
        public List<string?> GetRolePermissions(List<long> roleIds)
        {
            using (var db = new KevinDbContext())
            {

                return db.Set<TRolePermission>().Where(r => r.IsDelete == false && roleIds.Contains(r.RoleId)).Select(r => r.PermissionId).Distinct().ToList() ?? new List<string?>();

            }
        }

        /// <summary>
        /// 获取用户所拥有的权限列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetUserPermissions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new();
            }
            using (var db = new KevinDbContext())
            {
                var permList = GetAllPermissionIds();
                var allRoleIds = GetUserRoleIds(userId);
                var permissions = GetRolePermissions(allRoleIds);
                var result = permList.ToDictionary(x => x, x => { return permissions.Contains(x); });
                return result;
            }
        }

        /// <summary>
        /// 验证权限
        /// </summary> 
        /// <returns></returns>
        public bool IsAccess(string permissionId, IHttpContextAccessor httpContext)
        {
            //超级管理员直接返回true
            if (CurrentUser.IsSuperAdmin)
            {
                return true;
            }
            var pers = new Dictionary<string, bool>();
            pers = GetUserPermissions(CurrentUser.UserId.ToString());
            if (pers.Count == 0 || !pers.ContainsKey(CurrentUser.TenantId+"/" + permissionId))
            {
                return false;
            }
            return pers[CurrentUser.TenantId + "/" + permissionId];
        }
    }
}
