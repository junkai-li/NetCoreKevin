using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Msg;
using kevin.AI.AgentFramework.Tools;
using Kevin.Common.Helper.DingDing.Msg;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    public class AIMsgService : IAIMsgService
    {

        public readonly IUserService userService;

        public readonly IAIShareInfoService aIShareInfoService;
        public AIMsgService(IHttpContextAccessor _httpContextAccessor, IUserService _userService, IAIShareInfoService _aIShareInfoService)
        {
            userService = _userService;
            aIShareInfoService = _aIShareInfoService;
        }

        public async Task<string> SendDDToMyMsg([Description("消息内容")][Required] string msgContent)
        {
            string correlationId = "";
            if (aIShareInfoService.GetData() != default)
            {
                var userId = aIShareInfoService.GetData().UserId;
                if (userId > 0)
                {
                    var userInfo = await userService.GetSysUserWhereId(userId.ToTryInt64());
                    if (userInfo != default)
                    {
                        correlationId = userInfo.CorrelationId ?? "";
                    }
                }
            }
            if (string.IsNullOrEmpty(correlationId))
            {
                var userInfo = await userService.GetCurrentUserInfo();
                if (string.IsNullOrEmpty(userInfo?.CorrelationId))
                {
                    return userInfo?.Name + "未关联到用户钉钉Id";
                }
                correlationId = userInfo?.CorrelationId ?? "";
            }
            if (string.IsNullOrEmpty(correlationId))
            {
                return "未找到关联到用户钉钉Id";
            }
            return new DingDingMsgHelper().RobotSendTextMessageToUsers("", new List<string> { correlationId }, $"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】\n {msgContent}");
        }

        public async Task<string> SendDDToUserMsg([Description("消息内容")][Required] string msgContent, [Description("发送用户名称")][Required] string userName)
        {
            var userInfo = await userService.GetSysUserWhereUserName(userName);
            if (userInfo == default)
            {
                return "用户名称不存在";
            }
            if (string.IsNullOrEmpty(userInfo?.CorrelationId))
            {
                return userInfo?.Name + "未关联到用户钉钉Id";
            }
            return new DingDingMsgHelper().RobotSendTextMessageToUsers("", new List<string> { userInfo.CorrelationId }, $"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】\n {msgContent}");
        }
    }
}
