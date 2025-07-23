using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.SemanticKernel
{
    internal class demo
    {
        /// <summary>
        // 执行完成服务
        /// </summary>
        public async Task Start()
        {

            var modelId = "glm-4-flash";
            var endpoint = "https://open.bigmodel.cn/api/paas/v4/";
            var apiKey = "";

            // 2. 创建一个OpenAI聊天完成的内核
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId,
                new Uri(endpoint),
                apiKey);
            // 2. 定义提示
            var prompt = """
                        你是家里面的智能语言家居助手。 
                        请根据以下主人的命令进行相关操作：{{$concept}}
                        """;

            // 3.添加企业组件
            // builder.Services.AddLogging(services => services.AddDebug().SetMinimumLevel(LogLevel.Trace));

            // 4.构建内核
            Kernel kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            // 5.添加一个插件（LightsPlugin类定义如下）
            // kernel.Plugins.AddFromType<LightsPlugin>("Lights");
            var explainFunction = kernel.CreateFunctionFromPrompt(prompt);
            // 6.开启规划 自动调用函数
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // 7.创建一个历史记录存储对话
            var history = new ChatHistory();

            // 8.发起一次多轮对话聊天服务
            string? userInput;
            do
            {

                //发送图片
                // byte[] bytes = File.ReadAllBytes("path/to/image.jpg"); 
                // the LLM on its required role.
                //var chatHistory = new ChatHistory("Your job is describing images.");

                // Add a user message with both the image and a question
                // about the image.
                //chatHistory.AddUserMessage(
                //[
                //    new TextContent("What’s in this image?"),
                //    new ImageContent(bytes, "image/jpeg"),
                //]);
                // Invoke the chat completion model.
                //var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory);

                // 9.收集用户输入
                Console.Write("User > ");
                userInput = Console.ReadLine();
                var arguments = new KernelArguments { { "concept", userInput } };
                var Functionresult = await kernel.InvokeAsync(explainFunction, arguments);
                Console.WriteLine("Assistant > " + Functionresult);
                //        // 10.添加用户输入
                //        history.AddUserMessage(userInput);

                //        // 11.获得AI的响应  非流式
                //        var result = await chatCompletionService
                //            .GetChatMessageContentAsync(
                //            history,
                //            executionSettings: openAIPromptExecutionSettings,
                //            kernel: kernel);

                //        // 12.输出结果
                ////Console.WriteLine("Assistant > " + result);

                //        var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
                //            chatHistory: history,
                //            kernel: kernel
                //        );
                //        Console.Write("Assistant >");
                //        await foreach (var chunk in response)
                //        {
                //            Console.Write(chunk);
                //        } 
                //        Console.WriteLine();
                //       // 13.将消息添加到聊天记录
                //       history.AddMessage(result.Role, result.Content ?? string.Empty);
            } while (userInput is not null);
        }
    }
}
