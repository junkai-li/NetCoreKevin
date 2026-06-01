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
        [DefaultValue(2048)]
        public int MaxAskPromptSize { get; set; } = 2048;
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
        [DefaultValue(2048)]
        public int AnswerTokens { get; set; } = 2048;

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
        /// 工具绑定列表
        /// </summary>
        public List<AIAppsBindSkillToolsDto> Tools { get; set; } = new List<AIAppsBindSkillToolsDto>();

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
        public int MaxRetries { get; set; } = 3;
        /// <summary>
        /// AI请求超时时间，单位分钟
        /// </summary>
        [Description("AI请求超时时间，单位分钟")]
        public int NetworkTimeout { get; set; } = 10; 

        /// <summary>
        /// 关联的绑定ID
        /// </summary>
        public List<string> BindIds { get; set; } = new List<string>();
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
        /// 内容长度限制，超过限制后会进行截断，默认30000字符（知识库，互联网搜索，AI工具内容，文件内容等，）
        /// </summary>
        [Description(" 内容长度限制，超过限制后会进行截断，默认30000字符（知识库，互联网搜索，AI工具内容，文件内容等，）")] 
        public int ContentLengthLimit { get; set; } = 30000;
    }
}
