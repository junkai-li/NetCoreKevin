using kevin.AI.AgentFramework.Agent;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace kevin.AI.AgentFramework.WorkFlows
{
    public class WorkFlowsAndAIAgentsDemo
    {
        private readonly IAgent agent;
        public WorkFlowsAndAIAgentsDemo(IAgent agent)
        {
            this.agent = agent;
        }
        public async Task Start()
        {  
            AIAgent frenchAgent = agent.CreateAIAgent(instructions: "您是一名翻译助理，负责将提供的文本翻译成粤语", name: "粤语翻译器");
            AIAgent spanishAgent = agent.CreateAIAgent(instructions: "您是一名翻译助理，负责将提供的文本翻译成繁体字", name: "繁体字翻译器");
            AIAgent englishAgent = agent.CreateAIAgent(instructions: "您是一名翻译助理，负责将提供的文本翻译成英语", name: "英语翻译器");
            // Build the workflow by adding executors and connecting them
            var workflow = new WorkflowBuilder(frenchAgent)
                .AddEdge(frenchAgent, spanishAgent)
                .AddEdge(spanishAgent, englishAgent)
                .Build();

            // Execute the workflow
            StreamingRun run = await InProcessExecution.StreamAsync(workflow, new ChatMessage(ChatRole.User, "你好，中国"));

            // Must send the turn token to trigger the agents.
            // The agents are wrapped as executors. When they receive messages,
            // they will cache the messages and only start processing when they receive a TurnToken.
            Dictionary<string, string> messages = new Dictionary<string, string>();
            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                if (evt is AgentRunUpdateEvent executorComplete)
                {
                    if (messages.ContainsKey(executorComplete.ExecutorId))
                    {
                        messages[executorComplete.ExecutorId] += executorComplete.Data?.ToString();
                    }
                    else
                    {
                        messages.Add(executorComplete.ExecutorId, executorComplete.Data?.ToString() ?? "");
                    }
                    Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
                }
            }
            foreach (var item in messages)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
}
