using Azure;
using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tool
{

    public class HttpClientFunction(IHttpClientFactory httpClientFactory, IAIAgentService aIAgentService)
    {
        private Dictionary<string, string> SeoTemplates = new Dictionary<string, string>() {
            { "https://cn.bing.com/search?q={0}","必应搜索"},
            { "https://www.baidu.com/s?wd={0}&ie=utf-8&rn=10","百度搜索"},
            { "https://kaifa.baidu.com/searchPage?wd={0}&ie=utf-8&rn=10", "百度开发搜索" },
            { "https://www.sogou.com/web?query={0}", "搜狗搜索" },
            { "https://www.so.com/s?q={0}", "360搜索" },
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
            输出内容应结构清晰、简洁明了、重点突出、符合用户搜索意图、需要标记信息来源

             # 重要规则：
             # 1. 只使用互联网查询信息来回答
             # 2. 如果互联网查询信息中没有相关信息，请明确告知用户
             # 3. 不要编造或推测互联网查询信息中没有的信息
             # 4. 回答要清晰、准确、有条理
             # 5. 可以引用信息来源


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

            var htmls = new List<string>();
            string result = "";
            foreach (var SeoTemplate in SeoTemplates)
            {
                try
                {
                    var html = await GetUrlSeoAsync(string.Format(SeoTemplate.Key, value));
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
                    // 1. 统一换行符
                    html = html.Replace("\r\n", "\n").Replace("\r", "\n");
                    // 2. 移除多余的空白字符（但保留单个空格和换行）
                    html = Regex.Replace(html, @"[ \t]+", " ");
                    html = Regex.Replace(html, @"\n{3,}", "\n\n");
                    // 3. 移除首尾空白
                    html = html.Trim();
                    htmls.AddRange(html + "\n 信息来源于: " + SeoTemplate.Value);
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
                    Temperature = (float)(30 / 100),
                    ResponseFormat = ChatResponseFormat.Text,
                },
            })).Item2;

            return "互联网查询信息:" + "\n" + (result ?? "无相关信息");
        }

        public async Task<string> GetUrlSeoAsync(string url)
        {
            // 1. 配置 Handler，必须开启自动解压，并允许自动重定向
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true, // 允许自动跟随重定向
                                          // 注意：如果遇到 SSL 错误，可以临时开启（生产环境不建议）
                                          // ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using (var http = new HttpClient(handler))
            {
                http.Timeout = TimeSpan.FromSeconds(30); // 设置超时时间
                // 2. 设置请求头
                http.DefaultRequestHeaders.Clear();
                // User-Agent 必须是最新且真实的浏览器标识
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                // Referer 非常重要，告诉百度你是从哪个页面跳转过来的（这里模拟从百度首页跳转）
                http.DefaultRequestHeaders.Add("Referer", "https://www.baidu.com/");

                // 百度对 Accept-Encoding 很敏感，必须包含 br (Brotli)
                http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
                try
                {
                    // 3. 获取响应
                    var response = await http.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    // 4. 【关键步骤】读取原始字节数组，而不是直接读字符串
                    byte[] byteResult = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                    // 5. 【关键步骤】尝试获取编码，如果获取不到则使用默认逻辑
                    Encoding encoding = null;

                    // 尝试从响应头的 Content-Type 获取编码 (例如: "text/html; charset=gbk")
                    string charset = response.Content.Headers.ContentType?.CharSet;
                    if (!string.IsNullOrEmpty(charset))
                    {
                        try
                        {
                            // 处理一下 charset 名称（有时可能是 "gb2312" 或 "utf-8"）
                            encoding = Encoding.GetEncoding(charset.Replace("\"", ""));
                        }
                        catch
                        {
                            // 如果编码名称无效，忽略，走下面的自动检测逻辑
                        }
                    }

                    // 如果响应头没有指定编码，或者指定的无效，我们需要“猜”编码
                    // 注意：这里需要引入 NuGet 包 Ude 来实现 C# 端的自动检测，
                    // 或者使用下面的简单逻辑（针对中文网页的常见情况）
                    if (encoding == null)
                    {
                        // 尝试用 UTF-8 解码
                        string utf8Str = Encoding.UTF8.GetString(byteResult);

                        // 简单的启发式检测：如果解码后的字符串包含乱码特征字符（如 �），则尝试 GBK
                        // 注意：这只是一个简单的判断，更准确的做法是用 Ude 库
                        if (utf8Str.Contains("�") || utf8Str.Contains("锟斤拷"))
                        {
                            // 如果 UTF-8 失败，对于中文网页，极大概率是 GBK 或 GB2312
                            try
                            {
                                encoding = Encoding.GetEncoding("GBK");
                            }
                            catch
                            {
                                encoding = Encoding.UTF8; // 回退到 UTF-8
                            }
                        }
                        else
                        {
                            encoding = Encoding.UTF8;
                        }
                    }

                    // 6. 使用确定的编码将字节转换为字符串
                    string result = encoding.GetString(byteResult);

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请求失败: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
