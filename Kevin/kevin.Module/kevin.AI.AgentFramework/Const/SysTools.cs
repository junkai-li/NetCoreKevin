using kevin.AI.AgentFramework.Tools;
using Microsoft.Extensions.AI;

namespace kevin.AI.AgentFramework.Const
{
    public class SysTools
    {
        public static Dictionary<string, AITool> Tools { get; set; } = new Dictionary<string, AITool>()
{
    {
        "AgentHttpClientTools.GetAsync",
        AIFunctionFactory.Create(AgentHttpClientTools.GetAsync, new AIFunctionFactoryOptions
        {
            Name = "GetAsync",
            Description = "通用 HTTP 工具 发送 GET 请求"
        })
    },
    {
        "AgentHttpClientTools.PostAsync",
        AIFunctionFactory.Create(AgentHttpClientTools.PostAsync, new AIFunctionFactoryOptions
        {
            Name = "PostAsync",
            Description = "通用 HTTP 工具 发送 POST 请求"
        })
    },
    {
        "AgentHttpClientTools.PutAsync",
        AIFunctionFactory.Create(AgentHttpClientTools.PutAsync, new AIFunctionFactoryOptions
        {
            Name = "PutAsync",
            Description = "通用 HTTP 工具 发送 PUT 请求"
        })
    },
    {
        "AgentHttpClientTools.DeleteAsync",
        AIFunctionFactory.Create(AgentHttpClientTools.DeleteAsync, new AIFunctionFactoryOptions
        {
            Name = "DeleteAsync",
            Description = "通用 HTTP 工具 发送 DELETE 请求"
        })
    },
    {
        "ShellTools.RunShell",
        AIFunctionFactory.Create(ShellTools.RunShell, new AIFunctionFactoryOptions
        {
            Name = "RunShell",
            Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令(Windows 用 cmd也可以执行bash相关命令，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。"
        })
    },
    {
        "PythonTools.RunPythonPy",
        AIFunctionFactory.Create(PythonTools.RunPythonPy, new AIFunctionFactoryOptions
        {
            Name = "RunPythonPy",
            Description = "执行Python脚本。 可以帮助Skills工具执行scripts中含有后缀.py脚本的能力 通过PythonNet库来调用Python.py的脚本,并返回执行脚本结果 如果执行返回结果为空或者报错 可以使用RunShell来提取脚本代码然后自行调整定义main函数使用RunPythonCode来执行"
        })
    },
    {
        "PythonTools.RunPythonCode",
        AIFunctionFactory.Create(PythonTools.RunPythonCode, new AIFunctionFactoryOptions
        {
            Name = "RunPythonCode",
            Description = "执行Python代码。"
        })
    },
    {
        "CommonTools.GetRuntimePlatform",
        AIFunctionFactory.Create(CommonTools.GetRuntimePlatform, new AIFunctionFactoryOptions
        {
            Name = "GetRuntimePlatform",
            Description = "获取系统。用于获取当前运行在什么系统平台上"
        })
    },
    {
        "CommonTools.GetDesktopPath",
        AIFunctionFactory.Create(CommonTools.GetDesktopPath, new AIFunctionFactoryOptions
        {
            Name = "GetDesktopPath",
            Description = "获取当前系统桌面路径。 用于获取当前用户的桌面路径"
        })
    },
    {
        "CommonTools.WriteTextToDesktop",
        AIFunctionFactory.Create(CommonTools.WriteTextToDesktop, new AIFunctionFactoryOptions
        {
            Name = "WriteTextToDesktop",
            Description = "输出文件到系统桌面。 用于把各种文件输出到桌面"
        })
    }
};
    }
}
