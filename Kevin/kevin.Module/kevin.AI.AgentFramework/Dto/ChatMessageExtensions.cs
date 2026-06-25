using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Dto
{
    public static class ChatMessageExtensions
    {
        /// <summary>
        /// 检查消息是否包含工具调用
        /// </summary>
        public static bool HasFunctionCalls(this ChatMessage message)
        {
            return message.Contents?.OfType<FunctionCallContent>().Any() == true;
        }

        /// <summary>
        /// 检查消息是否包含工具响应
        /// </summary>
        public static bool HasFunctionResults(this ChatMessage message)
        {
            return message.Contents?.OfType<FunctionResultContent>().Any() == true;
        }

        /// <summary>
        /// 获取所有工具调用ID
        /// </summary>
        public static List<string> GetFunctionCallIds(this ChatMessage message)
        {
            return message.Contents?
                .OfType<FunctionCallContent>()
                .Select(fc => fc.CallId)
                .ToList() ?? new List<string>();
        }

        /// <summary>
        /// 获取所有工具响应ID
        /// </summary>
        public static List<string> GetFunctionResultIds(this ChatMessage message)
        {
            return message.Contents?
                .OfType<FunctionResultContent>()
                .Select(fr => fr.CallId)
                .ToList() ?? new List<string>();
        }
    }
}
