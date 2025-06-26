using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.MCP.Server.Tools
{
    [McpServerToolType]
    public sealed class MyTool
    {
        [McpServerTool, Description("Kevin_MCP_Server client.")]
        public static string Echo(string phone, string message)
        {
            return "hello " + phone + " 你好啊 欢迎使用mcp服务 message : " + message;
        }
    }
}
