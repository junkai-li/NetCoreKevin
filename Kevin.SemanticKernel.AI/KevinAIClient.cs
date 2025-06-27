using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.SemanticKernel
{
    public class KevinAIClient : IKevinAIClient
    {
        public ChatHistory history = new ChatHistory();
        private readonly Kernel _kernel;
        public KevinAIClient(Kernel kernel)
        {
            _kernel = kernel;
            history.AddSystemMessage("你是NetCoreKevin框架的通用聊天助手");
        }

        public string SendMsg(string msg)
        {
            history.AddUserMessage(msg);
            var arguments = new KernelArguments { { "userMessage", msg } };
            // 获取聊天函数
            var chatFunction = _kernel.Plugins.GetFunction("kevinai", "msg"); ;
            FunctionResult result = chatFunction.InvokeAsync(_kernel, arguments).Result;
            var data = result.ToString();
            history.AddAssistantMessage(data);
            return data;
        }
    }
}
