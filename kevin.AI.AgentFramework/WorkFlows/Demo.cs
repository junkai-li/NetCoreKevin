using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.WorkFlows
{
    public class Demo
    {
        public async Task Start()
        {
            //            WorkflowBuilder 构造函数接受起始执行器
            //AddEdge() 创建从大写到反向的定向连接
            //WithOutputFrom() 指定执行程序生成最终工作流输出
            //Build() 创建不可变工作流
            // Create the executors
            UppercaseExecutor uppercase = new();
            ReverseTextExecutor reverse = new();

            // Build the workflow by connecting executors sequentially
            WorkflowBuilder builder = new(uppercase);
            builder.AddEdge(uppercase, reverse).WithOutputFrom(reverse);
            var workflow = builder.Build();

            // Execute the workflow with input data
            Run run = await InProcessExecution.RunAsync(workflow, "Hello, World!");
            foreach (WorkflowEvent evt in run.NewEvents)
            {
                if (evt is ExecutorCompletedEvent executorComplete)
                {
                    Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
                }
            }
        }
    }
    /// <summary>
    /// First executor: converts input text to uppercase. 创建大写执行程序
    /// </summary>
    public sealed class UppercaseExecutor() : ReflectingExecutor<UppercaseExecutor>("UppercaseExecutor"),
        IMessageHandler<string, string>
    {
        public ValueTask<string> HandleAsync(string input, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            // Convert input to uppercase and pass to next executor
            return ValueTask.FromResult(input.ToUpper());
        }
    }

    /// <summary>
    /// Second executor: reverses the input text and completes the workflow. 创建反向文本执行程序
    /// </summary>
    internal sealed class ReverseTextExecutor() : ReflectingExecutor<ReverseTextExecutor>("ReverseTextExecutor"),
        IMessageHandler<string, string>
    {
        public ValueTask<string> HandleAsync(string input, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            // Reverse the input text
            return ValueTask.FromResult(new string(input.Reverse().ToArray()));
        }
    }
}
