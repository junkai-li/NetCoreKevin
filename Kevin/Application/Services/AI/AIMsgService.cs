using kevin.AI.AgentFramework.Interfaces.Msg;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.FileStorage;
using Kevin.Common.Extension;
using Kevin.Common.Helper.DingDing.Msg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kevin.Application.Services.AI
{
    public class AIMsgService : IAIMsgService
    {
        private object? _data { get; set; }

        public void InitData(object data)
        {
            _data = data;
        }
        public readonly IUserService userService;
        public AIMsgService(IHttpContextAccessor _httpContextAccessor, IUserService _userService)
        {
            userService = _userService;
        }

        public string SendDDToMyMsg([Description("消息内容")][Required] string msgContent)
        {
            string correlationId = "";
            if (_data != default)
            {
                var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                var userId = jsonDoc.RootElement.GetProperty("UserId").GetString();
                if (!string.IsNullOrEmpty(userId))
                {
                    var userInfo = userService.GetSysUserWhereId(userId.ToTryInt64());
                    if (userInfo != default)
                    {
                        correlationId = userInfo.CorrelationId ?? "";
                    }
                }
            }
            if (string.IsNullOrEmpty(correlationId))
            {
                var userInfo = userService.GetCurrentUserInfo();
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
            return new DingDingMsgHelper().SendMessage_ByDingDingURLText(correlationId, $"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】\n {msgContent}").ToJson();
        }

        public string SendDDToUserMsg([Description("消息内容")][Required] string msgContent, [Description("发送用户名称")][Required] string userName)
        {
            var userInfo = userService.GetSysUserWhereUserName(userName);
            if (userInfo == default)
            {
                return "用户名称不存在";
            }
            if (string.IsNullOrEmpty(userInfo?.CorrelationId))
            {
                return userInfo?.Name + "未关联到用户钉钉Id";
            }
            return new DingDingMsgHelper().SendMessage_ByDingDingURLText(userInfo.CorrelationId, $"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】\n {msgContent}").ToJson();
        }
    }
}
