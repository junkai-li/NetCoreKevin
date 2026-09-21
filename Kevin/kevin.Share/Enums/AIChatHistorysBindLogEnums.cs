using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Enums
{
    public enum AIChatHistorysBindLogEnums
    {
        //知识库
        Kmss = 1,
        //web搜索
        WebSeo = 2,
        //提示词
        SystemPrompt=3,
        //文件内容
        FileContent=4,
        //推荐问题（存 AI 回复行上的 JSON 字符串数组，前端当追问建议用，不进上下文）
        RecommendQuestions=5
    }
}
