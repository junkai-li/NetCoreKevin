using kevin.Domain.BaseDatas;
using kevin.Domain.Kevin;
using Kevin.Common.App;
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
           new TUser { Id = Guid.NewGuid(), Name = "admin",NickName="admin", PassWord = "admin",Email="admin",Phone="admin", CreateTime = DateTime.Now, UpdateTime = DateTime.Now,IsSuperAdmin=true,RoleId=TRoleBaseData.Id,TenantId=TenantHelper.GetSettingsTenantId()}
        };
    }
}
