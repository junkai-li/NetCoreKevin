using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Dto
{
    public class AIShareInfoDto
    {
        /// <summary>
        /// 聊天对话ID
        /// </summary>
        public long AIChatsId = 0;

        /// <summary>
        /// 聊天记录ID
        /// </summary>
        public long AIChatHistorysId = 0;

        /// <summary>
        /// 智能体ID
        /// </summary>
        public long AIAppsId { get; set; } = 0;

        /// <summary>
        /// 租户iD
        /// </summary>
        public int TenantId { get; set; } = 0;

        /// <summary>
        /// 是否开启安全拦截，默认true，开启安全拦截后，AI工具调用时会进行安全拦截，防止敏感信息泄露
        /// </summary>
        public bool IsSecurityIntercept { get; set; } = true;
        public long UserId { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 聊天消息限制条数，默认100条
        /// </summary>
        public int ChatMessageLimit { get; set; } = 100;

        /// <summary>
        ///请求授权的域名，多个域名用逗号分隔，默认允许所有域名
        /// </summary>
        public string AuthorizedDomains { get; set; } = "*";

        /// <summary>
        ///请求授权的域名，多个域名用逗号分隔，默认允许所有域名
        /// </summary>
        public List<string> AuthorizedDomainsList { get; set; } = new List<string>();

        /// <summary>
        /// 其他参数
        /// </summary>
        public object? RequestData { get; set; } = default;

        /// <summary>
        /// 内容长度限制，0表示不限制，
        /// </summary>
        public int ContentLengthLimit = 0;
    }
}
