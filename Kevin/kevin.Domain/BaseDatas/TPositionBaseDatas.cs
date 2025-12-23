using kevin.Domain.Configuration;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Share.Enums;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TPositionBaseDatas
    {
        public static List<TPosition> Data { get; set; } = new List<TPosition>()
        {
           new TPosition() {
               Id=4514141354257227771,
               Name="CEO",
               Code="NET-CEO",
               Description="CEO",
               Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TPosition() {
               Id=4514141354257227371,
               Name="CTO",
               Code="NET-CTO",
               Description="CTO",
                 Status=OrganizationalStatus.Active,
               ParentId=4514141354257227771,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TPosition() {
               Id=4514141352257227371,
               Name="NET开发人员",
               Code="NET-NET",
               Description="NET",
               ParentId=4514141354257227371,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TPosition() {
               Id=4514141324257227371,
               Name="JAVA开发人员",
               Code="NET-JAVA",
               Description="JAVA",
               ParentId=4514141354257227371,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
             new TPosition() {
               Id=4514141354257217771,
               Name="CHO",
               Code="NET-CHO",
               Description="CHO",
               ParentId=4514141354257227771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
              new TPosition() {
               Id=4514141354252217771,
               Name="招聘",
               Code="NET-ZP",
               Description="招聘",
               ParentId=4514141354257217771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
               new TPosition() {
               Id=4514141352512124771,
              Name="行政",
               Code="NET-XZ",
               Description="行政",
               ParentId=4514141354257217771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },

        };
    }
}
