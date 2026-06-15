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
using System.Threading.Tasks;

namespace kevin.Application.Services.AI
{
    public class AIMsgService : IAIMsgService
    {
        public readonly IUserService userService;
        public AIMsgService(IHttpContextAccessor _httpContextAccessor, IUserService _userService)
        {
            userService = _userService;
        }

        public string SendDDToMyMsg([Description("消息内容")][Required] string msgContent)
        {
            var userInfo = userService.GetCurrentUserInfo();
            if (string.IsNullOrEmpty(userInfo?.CorrelationId))
            {
                return userInfo?.Name + "未关联到用户钉钉Id";
            }
            return new DingDingMsgHelper().SendMessage_ByDingDingURLText(userInfo.CorrelationId, $"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】\n {msgContent}").ToJson();
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
