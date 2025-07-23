using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin_ESL
{
    public interface IESLClient
    {
        /// <summary>
        /// 链接
        /// </summary>
        void Connect();
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string Authenticate(string password);
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pand"></param>
        /// <returns></returns>
        string ApiCommand(string args, string pand = "OK");
        /// <summary>
        /// 发送命令并返回值
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pand"></param>
        /// <returns></returns>
        string ApiCommandReslut(string args);
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pand"></param>
        /// <returns></returns>
        string Command(string args, string pand = "OK"); 
        void GetAllMsgLog();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        void SendData(string data);
        /// <summary>
        /// 处理后的返回值
        /// </summary>
        /// <returns></returns>
        string RecolectResponse();
        /// <summary>
        /// 返回值
        /// </summary>
        /// <param name="contentLenght"></param>
        /// <param name="isstringutf"></param>
        /// <returns></returns>
        string ReceiveData(int contentLenght = 1, bool isstringutf = true);
        /// <summary>
        /// 关闭
        /// </summary>
        void Dispose();
        /// <summary>
        /// 开启监控
        /// </summary>
        void StartEnableMonitoring(Action<string> eventCallback);
        /// <summary>
        /// 重连
        /// </summary>
        void socketConti();
        /// <summary>
        /// 获取通道是否可用
        /// </summary>
        /// <param name="seo"></param>
        /// <returns></returns>
        bool GetChannelsIsOk(string seo);

        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <returns></returns>
        string GetChannelsData();
    }
}
