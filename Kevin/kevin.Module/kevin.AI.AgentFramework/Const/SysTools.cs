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
    } 
};
    }
}
