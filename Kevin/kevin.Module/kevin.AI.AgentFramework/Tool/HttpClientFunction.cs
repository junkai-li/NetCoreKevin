using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tool
{

    public class HttpClientFunction(IHttpClientFactory httpClientFactory, IAIAgentService aIAgentService)
    {
        private List<string> SeoTemplates = new List<string>() {
        "https://cn.bing.com/search?q={0}",
        "https://www.baidu.com/s?wd={0}",
        "https://kaifa.baidu.com/searchPage?wd={0}",
        "https://www.sogou.com/web?query={0}",
        "https://www.so.com/s?q={0}",
         };

        private const string SystemTemplate =
            @" 
            角色： 
            你是一款专业的搜索引擎助手，专门负责将HTML内容转换为结构化的Markdown格式。你的核心职责是精确提取HTML标签中的关键信息，并将其转换为清晰、规范的Markdown文档，同时保持内容的完整性和准确性。

            能力：
            HTML解析能力‌：能够准确识别和解析各种HTML标签（如标题、段落、列表、链接、表格等），并将其转换为对应的Markdown语法格式。
            信息提取与总结‌：具备强大的信息筛选能力，能够从复杂的HTML结构中提取核心内容，过滤冗余信息，生成简洁明了的文本摘要。
            智能过滤机制‌：能够根据用户的具体需求，自动识别并忽略与查询无关的内容，确保输出结果的高度相关性。
            格式标准化处理‌：严格按照Markdown语法规范进行格式转换，确保生成文档的可读性和兼容性。

            操作指南：
            完整性要求‌：处理用户提供的完整HTML文档或代码片段时，需要全面分析所有相关标签元素。
            关键信息优先‌：重点关注标题、正文段落、重要列表项等核心内容，适度简化装饰性或次要元素。
            即时响应机制‌：完成信息处理和格式转换后，立即输出结果，无需等待用户确认或进行额外的交互询问。
            准确性保障‌：确保转换过程中不丢失原文的核心含义，保持信息传递的准确性和完整性。

            输出标准：
            严格遵循HTML到Markdown的转换规范
            保持原文信息的完整性与准确性
            对不同HTML标签采用相应的Markdown转换规则
            避免添加任何主观解释或额外说明
            输出内容应结构清晰、简洁明了、重点突出、符合用户搜索意图、需要标记信息来源于搜索引擎

             # 重要规则：
             # 1. 只使用互联网查询信息来回答
             # 2. 如果互联网查询信息中没有相关信息，请明确告知用户
             # 3. 不要编造或推测互联网查询信息中没有的信息
             # 4. 回答要清晰、准确、有条理
             # 5. 可以引用信息来源于搜索引擎来源


            代码说明： 
            该提示词扩写后明确了智能体在HTML到Markdown转换过程中的具体职责和操作规范
            强化了信息提取的准确性和格式转换的标准性要求
            增加了智能过滤和关键信息识别的能力描述
            完善了输出标准和质量保障机制
            保持了原有提示词的核心功能定位，同时增强了可操作性和专业性";
        /// <summary>
        /// 搜索用户提出的问题
        /// </summary> 
        public async Task<string> GetSeoAsync(string value, string aiEndPoint, string aiModelName, string aiModelKey)
        {
            var http = httpClientFactory.CreateClient(nameof(HttpClientFunction));
            List<string> htmls = new List<string>();
            string result = "";

            foreach (var SeoTemplate in SeoTemplates)
            {
                try
                {
                    var html = await http.GetStringAsync(string.Format(SeoTemplate, value)).ConfigureAwait(false);
                    var scriptRegex = new Regex(@"<script[^>]*>[\s\S]*?</script>");
                    var styleRegex = new Regex(@"<style[^>]*>[\s\S]*?</style>");
                    var commentRegex = new Regex(@"<!--[\s\S]*?-->");
                    var headRegex = new Regex(@"<head[^>]*>[\s\S]*?</head>");
                    var tagAttributesRegex = new Regex(@"<(\w+)(?:\s+[^>]*)?>");
                    var emptyTagsRegex = new Regex(@"<(\w+)(?:\s+[^>]*)?>\s*</\1>");
                    html = scriptRegex.Replace(html, "");
                    html = styleRegex.Replace(html, "");
                    html = commentRegex.Replace(html, "");
                    html = headRegex.Replace(html, "");
                    html = tagAttributesRegex.Replace(html, "<$1>");
                    html = emptyTagsRegex.Replace(html, "");
                    htmls.AddRange(html + "\n 信息来源于搜索引擎: " + SeoTemplate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                } 
            }
            result = (await aIAgentService.CreateOpenAIAgentAndSendMSG($"用户搜索意图问题:{value} \n 互联网搜索信息如下：" + string.Join("\n", htmls) + "\n" + " 请你根据用户搜索意图问题来进行提取总结", aiEndPoint, aiModelName, aiModelKey, new ChatClientAgentOptions
            {
                Name = " 你是一款专业的搜索引擎助手，专门负责将HTML内容转换为结构化的Markdown格式。你的核心职责是精确提取HTML标签中的关键信息，并将其转换为清晰、规范的Markdown文档，同时保持内容的完整性和准确性。",
                Instructions = SystemTemplate,
                Description = SystemTemplate,
                ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                {
                    MaxOutputTokens = 2048,
                    Temperature = (float)(30 / 100),
                    ResponseFormat = ChatResponseFormat.Text,
                },
            })).Item2;

            return "互联网查询信息:" + "\n" + (result ?? "无相关信息");
        }
    }
}
