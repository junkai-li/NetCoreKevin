using kevin.Domain.Configuration;
using kevin.Domain.Entities.AI;
using kevin.Domain.Share.Enums;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TAISkillToolManagementBaseDatas
    {
        public static List<TAISkillToolManagement> Data { get; set; } = new List<TAISkillToolManagement>()
        {
             new TAISkillToolManagement() {
               Id=4514141254257227711,
               Name="发送 GET 请求",
               ClassMethod="AgentHttpClientTools.GetAsync",
               Description="通用 HTTP 工具 发送 GET 请求",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
            new TAISkillToolManagement() {
               Id=4514141254257227712,
               Name="发送 POST 请求",
               ClassMethod="AgentHttpClientTools.PostAsync",
               Description="通用 HTTP 工具 发送 POST 请求",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
            new TAISkillToolManagement() {
               Id=4514141254257227713,
               Name="发送 PUT 请求",
               ClassMethod="AgentHttpClientTools.PutAsync",
               Description="通用 HTTP 工具 发送 PUT 请求",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
            new TAISkillToolManagement() {
               Id=4514141254257227714,
               Name="发送 DELETE 请求",
               ClassMethod="AgentHttpClientTools.DeleteAsync",
               Description="通用 HTTP 工具 发送 DELETE 请求",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
              new TAISkillToolManagement() {
               Id=4514141254257227715,
               Name="执行 Shell 命令",
               ClassMethod="ShellTools.RunShell",
               Description="通过操作系统原生 Shell 执行命令",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
            new TAISkillToolManagement() {
               Id=4514141254257227717,
               Name="执行Python代码",
               ClassMethod="PythonTools.RunPythonCode",
               Description="执行Python代码",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
             new TAISkillToolManagement() {
               Id=4514141254257227718,
               Name="获取系统",
               ClassMethod="CommonTools.GetRuntimePlatform",
               Description="用于获取当前运行在什么系统平台上",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
              new TAISkillToolManagement() {
               Id=4514141254257227719,
               Name="获取当前系统桌面路径。",
               ClassMethod="CommonTools.GetDesktopPath",
               Description="获取当前系统桌面路径。 用于获取当前用户的桌面路径",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
               new TAISkillToolManagement() {
               Id=4514141254257227720,
               Name="输出文件到系统桌面",
               ClassMethod="CommonTools.WriteTextToDesktop",
               Description="用于把各种文件输出到桌面",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
           new TAISkillToolManagement() {
               Id=4514141254257227721,
               Name="创建或更新一个周期性自动任务",
               ClassMethod="iKevinAITasksService.AddOrUpdateCronTask",
               Description="创建或更新一个周期性自动任务",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
           new TAISkillToolManagement() {
               Id=4514141254257227731,
               Name="创建一次性任务",
               ClassMethod="iKevinAITasksService.AddOnceTask",
               Description="创建一个一次性任务：在指定的未来时间点执行一次后自动结束，不会重复执行，也无需移除",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },

             new TAISkillToolManagement() {
               Id=4514141254257227722,
               Name="移除周期性任务",
               ClassMethod="iKevinAITasksService.RemoveCronTask",
               Description="移除周期性任务",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },

               new TAISkillToolManagement() {
               Id=4514141254257227723,
               Name="立即触发某个周期性任务一次",
               ClassMethod="iKevinAITasksService.TriggerCronTask",
               Description="立即触发某个周期性任务一次   ",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },

              new TAISkillToolManagement() {
               Id=4514141254257227724,
               Name="获取我的所有周期性任务列表",
               ClassMethod="iKevinAITasksService.GetTaskList",

               Description="获取我的所有周期性任务列表",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
                 new TAISkillToolManagement() {
               Id=4514143214257227724,
               Name="发送钉钉消息给其他用户",
               ClassMethod="AIMsgService.SendDDToUserMsg",

               Description="发送钉钉消息给其他用户",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
                new TAISkillToolManagement() {
               Id=4514141254257227725,
               Name="expense-report",
               ClassMethod="",
               Description="费用报销Skill",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Skill,
               CreateUserId=TUserBaseData.TUsers[0].Id
           },  new TAISkillToolManagement() {
               Id=4514141254257227726,
               Name="system-ops",
               ClassMethod="",
               Description="系统运维诊断Skill",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Skill,
               CreateUserId=TUserBaseData.TUsers[0].Id
           }, new TAISkillToolManagement() {
               Id=4514141251257327726,
               Name="豆包联网搜索Global版本",
               ClassMethod="WebSearchEngine.DoubaoSearchGlobalAsync",
               Description="豆包联网搜索Global版本",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           }, new TAISkillToolManagement() {
               Id=4514112251217327726,
               Name="豆包联网搜索Custom版本",
               ClassMethod="WebSearchEngine.DoubaoSearchCustomAsync",
               Description="豆包联网搜索Custom版本",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,
               SkillToolType=AISkillToolTypeEnums.Tool,
               CreateUserId=TUserBaseData.TUsers[0].Id
           }
        };
    }
}
