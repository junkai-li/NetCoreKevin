using kevin.Domain.Configuration;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TDictionaryBaseDatas
    {
        public static List<TDictionary> Data { get; set; } = new List<TDictionary>()
        {
           new TDictionary() {
               Id=4514140354257227771,Key="上传文件限制50MB",Value="50",
               Type="UploadFileLimit",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id
           },
           new TDictionary() {
               Id=4514140354257227772,Key="PasswordRegex",
               Value="^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$|^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$",
               Type="LoginSecurity",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id,Sort=1,Remarks="登录密码复杂度正则：数字/大写字母/小写字母/特殊符号中至少三类，且长度>=8"
           },
           new TDictionary() {
               Id=4514140354257227773,Key="AccountFailedCount",Value="5",
               Type="LoginSecurity",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id,Sort=2,Remarks="同账号密码连续错误次数阈值，达到后锁定账号"
           },
           new TDictionary() {
               Id=4514140354257227774,Key="AccountLockMinutes",Value="15",
               Type="LoginSecurity",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id,Sort=3,Remarks="账号锁定时长（分钟）"
           },
           new TDictionary() {
               Id=4514140354257227775,Key="IpFailedCount",Value="50",
               Type="LoginSecurity",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id,Sort=4,Remarks="同一IP密码连续错误次数阈值，达到后锁定IP"
           },
           new TDictionary() {
               Id=4514140354257227776,Key="IpLockMinutes",Value="30",
               Type="LoginSecurity",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers[0].Id,Sort=5,Remarks="IP锁定时长（分钟）"
           },

        };
    }
}
