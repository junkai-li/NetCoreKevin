using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;

namespace kevin.AI.AgentFramework.Agent.ChatMessageStoreCom
{
    /// <summary>
    /// 将聊天历史记录存储在第三方存储中
    /// </summary>
    public class VectorChatMessageStoreDemo
    {
        private readonly AISetting AIConfig; 
        public VectorChatMessageStoreDemo(IOptionsMonitor<AISetting> config)
        {
            AIConfig = config.CurrentValue;
        }
        public async Task Start()
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(AIConfig.AIUrl);
            var ai = new OpenAIClient(new ApiKeyCredential(AIConfig.AIKeySecret), openAIClientOptions);
            ///运行代理
            AIAgent agent = ai.GetChatClient(AIConfig.AIDefaultModel).CreateAIAgent(new ChatClientAgentOptions
            {
                Name = "睡前故事智能体",
                Instructions = "你是一个会讲幼儿睡前故事的智能体",
                ChatMessageStoreFactory = ctx =>
                {
                    // Create a new chat message store for this agent that stores the messages in a vector store.
                    return new VectorChatMessageStore(
                       new InMemoryVectorStore(),
                       ctx.SerializedState,
                       ctx.JsonSerializerOptions);
                }
            });
            // 8.发起一次多轮对话聊天服务
            string? userInput;
            do
            {
                // 9.收集用户输入
                Console.Write("User > ");
                userInput = Console.ReadLine();
                Console.WriteLine(await agent.RunAsync(userInput ?? ""));
            } while (userInput is not null);
        }
    }
}
