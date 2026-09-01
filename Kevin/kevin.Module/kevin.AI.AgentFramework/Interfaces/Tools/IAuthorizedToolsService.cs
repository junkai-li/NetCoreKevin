using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    public interface IAuthorizedToolsService  
    {
        [Description("获取授权登录代码：当使用Python，Shell工具，http工具发起Http请求时，需要先获取授权代码， 返回授权码：输出JSON明确指示Token值和放置位置（URL参数或Headers） 失败异常返回以 ❌ 开头的错误信息")]
        Task<string> GetUrlAuthorizedCodeAsync([Description("传入完整的请求url如：https://ksiaa.com/api/product/lists")][Required] string url); 
    }
}
