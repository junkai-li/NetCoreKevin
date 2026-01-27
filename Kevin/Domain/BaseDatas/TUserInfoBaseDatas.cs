using kevin.Domain.Configuration;
using kevin.Domain.Share.Enums;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{ 
    public class TUserInfoBaseDatas
    { 
        public static List<TUserInfo> GetData()
        {
            var data = new List<TUserInfo>();
            foreach (var item in TUserBaseData.TUsers)
            {
                data.Add(new TUserInfo
                {
                    Id = 4514140314251221771,
                    UserId = item.Id,
                    DepartmentId = TDepartmentBaseDatas.Data[0].Id,
                    TenantId = TenantHelper.GetSettingsTenantId().ToTryInt32(),
                    CreateUserId = item.Id,
                    EmployeeStatus= EmployeeStatus.OnDuty,
                    HireDate= DateTime.Parse("2020-01-01 00:00:01"),
                    Sex=true, 
                    Signature ="你好.NET",
                    CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
                    EmployeeNo= "NetCoreKevin-00001"
                });
            }
            return data;
        }
    }
}
