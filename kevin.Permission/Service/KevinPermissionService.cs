using kevin.Domain.Share.Interfaces;
using Kevin.Common.App;
using Microsoft.AspNetCore.Http;
using Repository.Database;

namespace Web.Actions
{
    public class KevinPermissionService : IKevinPermissionService
    {

        public List<string> GetAllPermissionIds()
        {
            using (var db = new dbContext())
            {
                return db.TPermission.Where(x => x.IsDelete == false).Select(x => x.Id).ToList();

            }
        }
        public List<Guid> GetUserRoleIds(string userId)
        {
            using var db = new dbContext();
            var user = db.TUser.Where(x => x.IsDelete == false && x.Id == Guid.Parse(userId)).FirstOrDefault();
            if (user != null)
            {
                var reild = user.RoleId;
                var data = new List<Guid>();
                data.Add(reild);
                return data;
            }
            else
            {
                // Handle the case where the user is not found or is deleted.
                // You might want to return an empty list or some other appropriate response.
                return new List<Guid>();
            }
        }
        public List<string> GetRolePermissions(List<Guid> roleIds)
        {
            using (var db = new dbContext())
            {

                return db.TRolePermission.Where(r => r.IsDelete == false && roleIds.Contains(r.RoleId)).Select(r => r.PermissionId).Distinct().ToList(); ;

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
            using (var db = new dbContext())
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
            var pers = new Dictionary<string, bool>();
            var userId = JwtToken.GetClaims("userid", httpContext);
            pers = GetUserPermissions(userId);
            if (pers.Count == 0 || !pers.ContainsKey(permissionId))
            {
                return false;
            }
            return pers[permissionId];
        }
    }
}
