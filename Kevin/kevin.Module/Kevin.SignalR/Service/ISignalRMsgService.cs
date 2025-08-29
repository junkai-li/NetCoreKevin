using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SignalR.Service
{
    /// <summary>
    /// 自行实现相关逻辑接口
    /// </summary>
    public interface ISignalRMsgService
    {
        /// <summary>
        /// 发送公告消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendPublicMsg(string msg);

        /// <summary>
        /// 私发信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendUserMsg(string connId, string msg);

        /// <summary>
        /// 获取用户链接id
        /// </summary>
        /// <returns></returns>
        string GetUserConnId(string userId);

        /// <summary>
        /// 获取用户组链接id
        /// </summary>
        /// <returns></returns>
        List<string> GetTenantConnIds(int TenantId);

        /// <summary>
        /// 发送多个人
        /// </summary>
        /// <param name="connIds"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendUsersMsg(List<string> connIds, string msg);
    }
}
