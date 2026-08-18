using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    public interface IWebSearchEngine
    {
        /// <summary>
        /// 豆包搜索
        /// </summary>
        /// <param name="query">关键词</param>
        /// <returns></returns>
        [Description("豆包联网搜索Global版本，覆盖全球站点，摘要长度可灵活控制，综合搜索效果更好。参数：query")]
        public Task<string> DoubaoSearchGlobalAsync([Description("关键词")] string query);

        /// <summary>
        /// 豆包搜索
        /// </summary>
        /// <param name="query">关键词</param>
        /// <returns></returns>
        [Description("豆包联网搜索Custom版本，时延低，控制更灵活，支持各行业高频搜索需求。参数：query")]
        public Task<string> DoubaoSearchCustomAsync([Description("关键词")] string query);
    }
}
