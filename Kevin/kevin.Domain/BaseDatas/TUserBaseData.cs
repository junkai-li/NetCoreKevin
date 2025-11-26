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
           new TUser { Id =4514140314251222771, Name = "admin",NickName="admin", PasswordHash = new HashHelper().SHA256Hash("admin"),Email="admin",Phone="admin", CreateTime = DateTime.Parse("2020-01-01 00:00:01"),IsSuperAdmin=true,TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32()}
        };
    }
}
