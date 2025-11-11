using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using kevin.AI.AgentFramework.Agent;

namespace kevin.AI.AgentFramework.WorkFlows
{
    public class AgentDemo
    {
        private readonly IAgent agent;
        public AgentDemo(IAgent agent)
        {
            this.agent = agent;
        }
        public async Task Start()
        {
            var chatClient = this.agent.GetChatClient();
            // Create the AI agents with specialized expertise
            ChatClientAgent physicist = new(
                chatClient,
                name: "物理学家",
                instructions: "你是物理学专家。你从物理学的角度回答问题。"
            );

            ChatClientAgent chemist = new(
                chatClient,
                name: "化学家",
                instructions: "你是化学专家。你从化学的角度回答问题。"
            );

            Console.WriteLine(await physicist.RunAsync("简单说一下,什么是温度？"));
            Console.WriteLine(await chemist.RunAsync("简单说一下,什么是温度？"));
            //创建启动执行程序
            var startExecutor = new ConcurrentStartExecutor();
            //创建聚合执行程序
            var aggregationExecutor = new ConcurrentAggregationExecutor();
            // Build the workflow by adding executors and connecting them
            var workflow = new WorkflowBuilder(startExecutor)
                .AddFanOutEdge(startExecutor, targets: [physicist, chemist])
                .AddFanInEdge(aggregationExecutor, sources: [physicist, chemist])
                .WithOutputFrom(aggregationExecutor)
                .Build();
            // Execute the workflow in streaming mode
            StreamingRun run = await InProcessExecution.StreamAsync(workflow, "简单说一下,什么是温度？");
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                if (evt is WorkflowOutputEvent output)
                {
                    Console.WriteLine($"工作流已完成，结果如下：\n{output.Data}");
                }
            }
            Console.WriteLine("结束");
        }

        /// <summary>
        /// Executor that starts the concurrent processing by sending messages to the agents.
        /// </summary>
        public sealed class ConcurrentStartExecutor() :
            ReflectingExecutor<ConcurrentStartExecutor>("ConcurrentStartExecutor"),
            IMessageHandler<string>
        {
            public async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
            {
                await context.SendMessageAsync(new ChatMessage(ChatRole.User, message));
            }
        }

        /// <summary>
        /// Executor that aggregates the results from the concurrent agents.
        /// </summary>
        public sealed class ConcurrentAggregationExecutor() :
            ReflectingExecutor<ConcurrentAggregationExecutor>("ConcurrentAggregationExecutor"),
            IMessageHandler<ChatMessage>
        {
            private readonly List<ChatMessage> _messages = [];

            /// <summary>
            /// Handles incoming messages from the agents and aggregates their responses.
            /// </summary>
            /// <param name="message">The message from the agent</param>
            /// <param name="context">Workflow context for accessing workflow services and adding events</param> 

            public async ValueTask HandleAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken = default)
            {
                this._messages.Add(message);

                if (this._messages.Count == 2)
                {
                    var formattedMessages = string.Join(Environment.NewLine,
                        this._messages.Select(m => $"{m.AuthorName}: {m.Text}"));
                    await context.YieldOutputAsync(formattedMessages);
                }
            }
        }
    }
}
