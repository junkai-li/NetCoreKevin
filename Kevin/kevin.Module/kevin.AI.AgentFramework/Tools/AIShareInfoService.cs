using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tools
{
    public class AIShareInfoService : IAIShareInfoService
    {
        private AIShareInfoDto data;
        public AIShareInfoDto GetData()
        {
            return data;
        }
        public void InitData(AIShareInfoDto data)
        {
            this.data = data;
            // 解析规则与 CommandGuardrails.IsUrlAuthorized / ParseDomainEntries 同一份：
            // 空、“*”、只含空白项都得到空名单，下游护栏因此不启用（全放行）
            this.data.AuthorizedDomainsList = CommandGuardrails.ParseDomainEntries(data.AuthorizedDomains);
        }
    }
}
