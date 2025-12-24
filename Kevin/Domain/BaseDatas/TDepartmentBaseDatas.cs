using kevin.Domain.Configuration;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Share.Enums;
using Kevin.Common.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.BaseDatas
{
    public class TDepartmentBaseDatas
    {
        public static List<TDepartment> Data { get; set; } = new List<TDepartment>()
        {
           new TDepartment() {
               Id=4514141254257227771,
               Name="NET科技有限公司",
               Code="NET",
               Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TDepartment() {
               Id=4514141354257227371,
               Name="开发部",
               Code="NET-DEV",
               Status=OrganizationalStatus.Active,
               ParentId=4514141254257227771,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TDepartment() {
               Id=4514141352227227371,
               Name=".NET部门",
               Code="NET-DEV-NET",
               ParentId=4514141354257227371,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
            new TDepartment() {
               Id=4514141324252227371,
               Name="JAVA部门",
               Code="NET-DEV-JAVA",
               ParentId=4514141354257227371,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
             new TDepartment() {
               Id=4514141354257217771,
               Name="人力部门",
               Code="NET-CHO-DEPT",
               ParentId=4514141254257227771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
              new TDepartment() {
               Id=4514141354252217771,
               Name="招聘部门",
               Code="NET-ZP",
               ParentId=4514141354257217771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },
               new TDepartment() {
               Id=4514141352512124771,
              Name="行政部门",
               Code="NET-XZ",
               ParentId=4514141354257217771,
                 Status=OrganizationalStatus.Active,
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },

        };
    }
}
