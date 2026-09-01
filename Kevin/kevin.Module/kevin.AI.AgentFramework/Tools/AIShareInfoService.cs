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
            this.data.AuthorizedDomainsList = new List<string>();
            if (!string.IsNullOrWhiteSpace(data.AuthorizedDomains) && data.AuthorizedDomains.Trim() != "*")
            {
                data.AuthorizedDomains.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()
                    .ForEach(domain => this.data.AuthorizedDomainsList.Add(domain));
            }
        }
    }
}
