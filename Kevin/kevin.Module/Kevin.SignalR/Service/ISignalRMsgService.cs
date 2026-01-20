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
        Task SendPublicMsg(string method, string msg);

        /// <summary>
        /// 私发信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendConnIdMsg(string method, string connId, string msg);

        /// <summary>
        /// 获取身份链接id
        /// </summary>
        /// <returns></returns>
        string GetIdentityConnId(string identityId);

        /// <summary>
        /// 获取租户链接id
        /// </summary>
        /// <returns></returns>
        List<string> GetTenantConnIds(int TenantId);

        /// <summary>
        /// 获取租户所有身份ids
        /// </summary>
        /// <param name="TenantId"></param>
        /// <returns></returns>
        List<string> GetTenantIdentityIds(int TenantId);

        /// <summary>
        /// 发送多个人
        /// </summary>
        /// <param name="connIds"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendConnIdsMsg(string method,List<string> connIds, string msg);

        /// <summary>
        ///指定身份发送消息
        /// </summary>
        /// <param name="method"></param>
        /// <param name="identityId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendIdentityIdMsg(string method, string identityId, string msg);

        /// <summary>
        ///指定多身份发送消息
        /// </summary>
        /// <param name="method"></param>
        /// <param name="identityIds"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendIdentityIdsMsg(string method, List<string> identityIds, string msg);
    }
}
