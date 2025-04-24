using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI
{
    public interface IAIClient
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">内容：聊天内容</param>
        /// <param name="model">使用模型：</param>
        /// <param name="systemMsg">系统内容：你是一个聊天机器人，请根据用户输入的内容进行回复</param>
        /// <returns></returns>
        Dto.AIReslutDto SendMsg(string msg, string model = "", string systemMsg = "");
    }
}
