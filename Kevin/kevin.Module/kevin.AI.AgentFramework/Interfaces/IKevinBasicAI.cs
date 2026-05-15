using System.ComponentModel;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IKevinBasicAI
    {
        [Description("获取NetCoreKevin框架的介绍信息")]
        public string GetNetCoreKevinInfo(string userId);
    }
}
