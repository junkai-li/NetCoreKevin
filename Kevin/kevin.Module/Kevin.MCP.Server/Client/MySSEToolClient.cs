using Kevin.AI.MCP.Server.Models;
using Microsoft.Extensions.Options;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kevin.AI.MCP.Server.Client
{
    public class MySseToolClient : IMySseToolClient
    {
        private IMcpClient? _client { get; set; }
        public MCPSseClientSetting mCPSSEClientSetting { get; set; }

        public MySseToolClient(IOptionsMonitor<MCPSseClientSetting> config)
        {
            mCPSSEClientSetting = config.CurrentValue;
            GetClientAsync();
        }
        public IMcpClient GetClientAsync()
        {
            if (_client == default)
            {
                var clientTransport = new SseClientTransport(new SseClientTransportOptions
                {
                    Endpoint = new Uri(mCPSSEClientSetting.Url),
                    Name = mCPSSEClientSetting.Name,
                    ConnectionTimeout = mCPSSEClientSetting.ConnectionTimeout,
                    UseStreamableHttp = mCPSSEClientSetting.UseStreamableHttp,
                    AdditionalHeaders = mCPSSEClientSetting.AdditionalHeaders,
                });
                _client = McpClientFactory.CreateAsync(clientTransport).Result;
            }
            return _client;
        }
        /// <summary>
        /// 获取工具列表
        /// </summary>
        /// <returns></returns>
        public Task<List<McpClientTool>> GetListToolsAsync()
        {
            return Task.FromResult(GetClientAsync().ListToolsAsync().Result.ToList());
        }
        /// <summary>
        ///  调用工具
        /// </summary>
        /// <param name="toolName">工具名称</param>
        /// <param name="arguments"></param>
        /// <param name="progress"></param>
        /// <param name="serializerOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CallToolResponse> CallToolAsync(string toolName,
        IReadOnlyDictionary<string, object?>? arguments = null,
        IProgress<ProgressNotificationValue>? progress = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetClientAsync().CallToolAsync(toolName, arguments, progress, serializerOptions, cancellationToken).Result);
        }
    }
}
