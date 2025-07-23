using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.SemanticKernel
{
    public interface IKevinAIClient
    {
        /// <summary>
        /// 发送消息询问框架问题
        /// </summary>
        /// <param name="msg">内容：聊天内容</param> 
        /// <returns></returns>
       string SendMsg(string msg);
    }
}
