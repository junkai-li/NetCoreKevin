using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.MCP.Server.Models
{
    public class MCPSseClientSetting
    {
       
        public MCPSseClientSetting(string Name, string Url, bool UseStreamableHttp, Dictionary<string, string>? AdditionalHeaders, TimeSpan ConnectionTimeout)
        {
            this.Name = Name;
            this.Url = Url;
            this.UseStreamableHttp = UseStreamableHttp;
            this.AdditionalHeaders = AdditionalHeaders;
            this.ConnectionTimeout = ConnectionTimeout;
        } 
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ////Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use "Streamable HTTP" for the transport rather than "HTTP with SSE". Defaults to false.
        /// <see href="https://modelcontextprotocol.io/specification/2025-03-26/basic/transports#streamable-http">Streamable HTTP transport specification</see>.
        /// <see href="https://modelcontextprotocol.io/specification/2024-11-05/basic/transports#http-with-sse">HTTP with SSE transport specification</see>.
        /// </summary>
        public bool UseStreamableHttp { get; set; }
        /// <summary>
        /// Gets custom HTTP headers to include in requests to the SSE server.
        /// </summary>
        /// <remarks>
        /// Use this property to specify custom HTTP headers that should be sent with each request to the server.
        /// </remarks>
        public Dictionary<string, string>? AdditionalHeaders { get; set; }
        /// <summary>
        /// Gets or sets a timeout used to establish the initial connection to the SSE server. Defaults to 30 seconds.
        /// </summary>
        /// <remarks>
        /// This timeout controls how long the client waits for:
        /// <list type="bullet">
        ///   <item><description>The initial HTTP connection to be established with the SSE server</description></item>
        ///   <item><description>The endpoint event to be received, which indicates the message endpoint URL</description></item>
        /// </list>
        /// If the timeout expires before the connection is established, a <see cref="TimeoutException"/> will be thrown.
        /// </remarks>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
