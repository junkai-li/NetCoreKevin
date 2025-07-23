
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;

namespace Kevin.AI.MCP.Server.Client
{
    public interface IMySseToolClient
    {
        /// <summary>
        /// Get the client
        /// </summary>
        /// <returns></returns>
        Task<IMcpClient> GetClientAsync();
        /// <summary>
        /// Get the tools
        /// </summary>
        /// <returns></returns>
        Task<List<McpClientTool>> GetListToolsAsync();

        /// <summary>
        /// Call a tool
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="arguments"></param>
        /// <param name="progress"></param>
        /// <param name="serializerOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CallToolResponse> CallToolAsync(string toolName,
       IReadOnlyDictionary<string, object?>? arguments = null,
       IProgress<ProgressNotificationValue>? progress = null,
       JsonSerializerOptions? serializerOptions = null,
       CancellationToken cancellationToken = default);
    }
}
