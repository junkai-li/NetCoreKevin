using kevin.Domain.BaseDatas;
using kevin.Domain.Kevin;
using Kevin.Common.App;
using Kevin.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Configuration
{

    /// <summary>
    /// 种子数据配置
    /// </summary>
    public class TUserBaseData
    {
        public static List<TUser> TUsers { get; set; } = new List<TUser>()
        {
           new TUser { Id =Guid.Parse("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), Name = "admin",NickName="admin", PasswordHash = new HashHelper().SHA256Hash("admin"),Email="admin",Phone="admin", CreateTime = DateTime.Now, UpdateTime = DateTime.Now,IsSuperAdmin=true,RoleId=TRoleBaseData.Id,TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32()}
        };
    }
}
