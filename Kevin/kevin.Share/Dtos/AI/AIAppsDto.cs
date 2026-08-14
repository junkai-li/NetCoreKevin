using kevin.Domain.Share.Dtos.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIAppsDto : CUD_User_Dto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        public string Describe { get; set; } = "";

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public string Icon { get; set; } = "windows";

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 会话模型ID
        /// </summary>
        [Required]
        public string? ChatModelID { get; set; }


        public string? RerankModelID { get; set; }

        /// <summary>
        /// 温度
        /// </summary> 
        [DefaultValue(70)]
        public double Temperature { get; set; } = 70f;

        /// <summary>
        /// 知识库ID
        /// </summary> 
        public long? KmsId { get; set; }

        /// <summary>
        /// API调用秘钥
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// 相似度
        /// </summary> 
        [DefaultValue(60)]
        public double Relevance { get; set; } = 60f;

        /// <summary>
        /// 提问最大token数
        /// </summary> 
        [DefaultValue(20480)]
        public int MaxAskPromptSize { get; set; } = 20480;
        /// <summary>
        /// 向量匹配数
        /// </summary> 
        [DefaultValue(3)]
        public int MaxMatchesCount { get; set; } = 3;


        [DefaultValue(20)]
        public int RerankCount { get; set; } = 20;
        /// <summary>
        /// 回答最大token数
        /// </summary> 
        [DefaultValue(20480)]
        public int AnswerTokens { get; set; } = 20480;

        /// <summary>
        /// 提示词绑定
        /// </summary>
        public long AIPromptID { get; set; }

        public virtual string? AIPromptName { get; set; }

        /// <summary>
        /// 输出消息类型 1.非流式文本 2.流式文本 3.图片 4.音频 5.视频 6.文件 7.链接 8.卡片 
        /// </summary>
        [Required]
        public int MsgType { get; set; } = 1;

        public void Check()
        {
            if (string.IsNullOrEmpty(this.ChatModelID))
            {
                throw new FieldValidationException("请选择会话模型");
            }
            if (this.AIPromptID == default || this.AIPromptID == 0)
            {
                throw new FieldValidationException("提示词不能为空");
            }
            if (this.MaxRetries < 1)
            {
                throw new FieldValidationException("AI请求最大重试次数最小为1");
            }
            if (this.NetworkTimeout < 1)
            {
                throw new FieldValidationException("AI请求超时时间最小为1分钟");
            }
        }

        /// <summary>
        /// 是否开启ai工具，开启后可以使用AI工具类技能，关闭后只能使用普通技能
        /// </summary>
        [Description("是否开启ai工具，开启后可以使用AI工具类技能")]
        public bool IsAITools { get; set; } = true;

        /// <summary>
        /// 是否开启aiMcp工具
        /// </summary>
        [Description("是否开启aiMcp工具")]
        public bool IsMcp { get; set; } = true;

        /// <summary>
        /// 工具绑定列表
        /// </summary>
        public List<AIAppsBindSkillToolsDto> Tools { get; set; } = new List<AIAppsBindSkillToolsDto>();

        /// <summary>
        /// Mcp绑定列表
        /// </summary>
        public List<AIAppsBindSkillToolsDto> Mcps { get; set; } = new List<AIAppsBindSkillToolsDto>();

        /// <summary>
        /// 是否开启Skill技能，开启后可以使用Skill技能
        /// </summary>
        [Description("是否开启Skill技能，开启后可以使用Skill技能")]
        public bool IsSkill { get; set; } = true;

        /// <summary>
        /// Skill技能绑定列表
        /// </summary>
        public List<AIAppsBindSkillToolsDto> Skills { get; set; } = new List<AIAppsBindSkillToolsDto>();

        /// <summary>
        /// 是否开启AI请求日志
        /// </summary>
        [Description("是否开启AI请求日志")]
        public bool IsHttpLog { get; set; } = false;

        /// <summary>
        /// AI请求最大重试次数
        /// </summary>
        [Description("AI请求最大重试次数")]
        public int MaxRetries { get; set; } = 2;
        /// <summary>
        /// AI请求超时时间，单位分钟
        /// </summary>
        [Description("AI请求超时时间，单位分钟")]
        public int NetworkTimeout { get; set; } = 30;

        /// <summary>
        /// 关联的绑定ID
        /// </summary>
        public List<string> BindIds { get; set; } = new List<string>();

        /// <summary>
        /// 关联的绑定AI工具类技能ID列表
        /// </summary>
        public List<string> AISkillsToolsBindIds { get; set; } = new List<string>();
        /// <summary>
        /// AI请求授权白名单 *为所有，逗号分隔多个域名前缀
        /// </summary>
        [Description("AI请求授权白名单 *为所有，逗号分隔多个域名前缀")]
        [DefaultValue("*")]
        [MaxLength(500)]
        public string AuthorizedDomains { get; set; } = "*";

        /// <summary>
        /// 对话消息数量的限制
        /// </summary>
        [Description("对话消息数量的限制，默认100条")]
        [DefaultValue(100)]
        public int ChatMessageLimit { get; set; } = 100;

        /// <summary>
        /// 是否开启AI思考过程流式展示日志，开启后可以记录AI思考过程日志，方便调试和优化
        /// </summary>
        [Description("是否开启AI思考过程流式展示日志")]
        public bool IsThinkingLog { get; set; } = true;


        /// <summary>
        /// 是否开启AI工具调用流式展示日志 ，开启后可以记录AI工具调用日志，方便调试和优化
        /// </summary>
        [Description("是否开启AI工具调用流式展示日志")]
        public bool IsToolLog { get; set; } = true;

        /// <summary>
        /// 内容长度限制，超过限制后会进行截断，默认50000字符（知识库，互联网搜索，AI工具内容，文件内容等，）
        /// </summary>
        [Description(" 内容长度限制，超过限制后会进行截断，默认50000字符（知识库，互联网搜索，AI工具内容，文件内容等，）")]
        public int ContentLengthLimit { get; set; } = 50000;

        /// <summary>
        /// 是否开启安全拦截开启后会对python脚本和shell命令内容进行安全拦截，防止输入敏感信息，默认开启
        /// </summary>
        [Description("是否开启安全拦截，开启后会对python脚本和shell命令内容进行安全拦截，防止输入敏感信息，默认开启")]
        public bool IsSecurityIntercept { get; set; } = true;

        [Description(" 用户轮次计数,默认10，超出后其余对话自动压缩")]
        [DefaultValue(10)]
        public int ConversationTurnsExceed { get; set; } = 10;
        /// <summary>
        /// 是否开启对话自动压缩，开启后会对历史对话，思考过程，工具结果，返回内容，自动压缩，默认不开启"
        /// </summary>
        [Description("是否开启对话自动压缩，开启后会对历史对话，思考过程，工具结果，返回内容，自动压缩，默认不开启")]
        public bool IsAIMessageCompaction { get; set; } = false;
        /// <summary>
        /// 是否开启自动获取对话自动压缩，开启后会对自动获取压缩对话，默认不开启，不开启则使用智能体工具方式获取对话历史记录"
        /// </summary>
        [Description("是否开启自动获取对话自动压缩，开启后会对自动获取压缩对话，默认不开启，不开启则使用智能体工具方式获取对话历史记录")]
        public bool IsAutoGetAIMessageCompaction { get; set; } = false;
        /// <summary>
        /// 自动压缩策略提示词
        /// </summary>
        [Description("自动压缩策略提示词")]
        public String AIMessageCompactionPrompt { get; set; } = @"# 任务：信息精炼与结构化提取 
                                                    请将下方【原始内容】压缩至原文篇幅的20%以内，并**严格按以下四个字段**输出摘要。  
                                                    每个字段都必须出现，若原文无对应信息，请标注“无”。

                                                    ## 必输字段（顺序固定）
                                                    1. **用户对话核心任务**：用户在此次交互中最主要的目标或诉求是什么？（一句话概括）
                                                    2. **核心结论**：从整体内容中提炼出的最终判断、结果或关键事实。
                                                    3. **核心思考过程**：推导出结论所依据的推理链条或关键分析步骤（简要逻辑流）。
                                                    4. **工具返回关键信息**：需要输出工具名称，外部工具、系统或数据源返回的重要信息（如路径、数值、状态等）。

                                                    ## 输出格式要求 
                                                    - 每项内容尽量精简，去除所有修饰性描述、举例和过渡句。
                                                    - 如某字段信息完全缺失，直接写“无”。

                                                    ## 示例输出（供参考）
                                                    - **用户对话核心任务**：查询文件保存位置。
                                                    - **核心结论**：文件已成功保存至桌面。
                                                    - **核心思考过程**：根据系统反馈，路径存在且写入权限正常。
                                                    - **工具返回关键信息**：调用“查询文件保存位置”工具，结果： `C:\Users\XXX\Desktop`。

                                                    ---

                                                    【原始内容】：";

        /// <summary>
        /// 模型返回格式，默认不配置，支持Json,Text
        /// </summary> 
        [Description("模型返回格式，默认不配置，支持Json,Text")]
        public string? ResponseFormat { get; set; } = "";

        /// <summary>
        /// 模型思考能力，默认不配置，支持None,Low,Medium,High,ExtraHigh
        /// </summary> 
        [Description("模型思考能力，默认不配置，支持0.None,1.Low,2.Medium,3.High,4.ExtraHigh")]
        public int? ReasoningEffort { get; set; }

        /// <summary>
        /// 模型思考输出，默认不配置，支持None,Summary,Full
        /// </summary>
        [Description("模型思考输出，默认不配置，支持0.None,1.Summary,2.Full")]
        public int? ReasoningOutput { get; set; }
    }
}
