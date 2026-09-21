using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 聊天记录
    /// </summary>
    public class AIChatHistorysDto : CD_User_Dto
    {
        /// <summary>
        /// 对话id
        /// </summary>
        public long AIChatsId { get; set; }
        public virtual string? AIChats { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary> 
        [Description("消息内容")]
        public String Content { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        [Description("发送是true  接收是false")]
        public Boolean IsSend { get; set; } = false;

        /// <summary>
        /// 1非流式 2.流式
        /// </summary>
        [Description("发送是true  接收是false")]
        public int MsgType { get; set; } = 1;


        /// <summary>
        /// 是否联网搜索
        /// </summary>
        [Description("是否联网搜索")]
        public Boolean IsOnlineSearch { get; set; } = false;
        /// <summary>
        ///AI思考过程
        /// </summary>
        [Description("思考过程")]
        public String? AIReasoningContent { get; set; }

        /// <summary>
        ///AI工具调用过程
        /// </summary>
        [Description("AI工具调用过程")]
        public String? AIToolsContent { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Description("文件名s,多个用,隔开")]
        public String? FileNames { get; set; }
        /// <summary>
        ///发送的文件urls
        /// </summary>
        [Description("发送的文件urls,多个用,隔开")]
        public String? ContentFileUrls { get; set; }

        /// <summary>
        /// 输入消耗的token数
        /// </summary>
        [Description("输入消耗的token数")]
        public long? InputTokenCount { get; set; }

        /// <summary>
        /// 输出消耗的token数
        /// </summary>
        [Description("输出消耗的token数")]
        public long? OutputTokenCount { get; set; }

        /// <summary>
        /// 总消耗的token数
        /// </summary>
        [Description("总消耗的token数")]
        public long? TotalTokenCount { get; set; }

        /// <summary>
        /// 缓存中读取的输入标记的数量 缓存的输入标记应计入<see cref="InputTokenCount"/>中。
        /// </summary>
        [Description("缓存中读取的输入标记的token数量")]
        public long? CachedInputTokenCount { get; set; }

        /// <summary>
        /// “推理”/“思考”标记的数量
        /// by the model.
        /// </summary>
        /// <remarks>
        /// 推理标记应计入<see cref="OutputTokenCount"/>中。
        /// </remarks>
        [Description("“推理”/“思考”token数量")]
        public long? ReasoningTokenCount { get; set; }

        /// <summary>
        /// 相关日志记录
        /// </summary> 
        public List<AIChatHistorysBindLogDto> aIChatHistorysBindLogs { get; set; } = new List<AIChatHistorysBindLogDto>();

        /// <summary>
        /// 发送状态：0.正常 1.失败(可重试) 2.重试中
        /// </summary>
        [Description("发送状态 0.正常 1.失败可重试 2.重试中")]
        public AIChatHistorysSendStatusEnums SendStatus { get; set; } = AIChatHistorysSendStatusEnums.Success;

        /// <summary>
        /// 失败原因，仅 <see cref="SendStatus"/> 非正常时有值
        /// </summary>
        [Description("失败原因")]
        public String? FailReason { get; set; }

        /// <summary>
        /// 该提问已重试次数
        /// </summary>
        [Description("已重试次数")]
        public int RetryCount { get; set; }

        /// <summary>
        /// 重试来源：前端点红色重试按钮时回传上次失败的提问记录Id，服务端据此废弃旧的失败问答并累加重试次数。
        /// <para>不落库，仅作为 <c>Add</c> 的入参使用，为 0 表示这是一次普通发送。</para>
        /// </summary>
        public long RetryOfId { get; set; }

        /// <summary>
        /// 是否开启推荐问题：本轮一开始就后台“二次问一次 AI”（只看用户提问，与本轮回答并行）生成 3 个追问建议，
        /// 回答结束后经流式通道 <c>recommendmsg</c> 事件下发。
        /// <para>
        /// 该开关本身不落库；推荐结果不写聊天记录正文，只以一条绑定日志（日志类型 5）挂在回复记录上：
        /// 既供前端历史回显，也会作为下一轮的“上一轮推荐追问”上下文回给模型，以便“用你推荐的第二个问题”这类指代能被理解。
        /// </para>
        /// </summary>
        [Description("是否开启推荐问题")]
        public Boolean IsRecommendQuestion { get; set; } = false;
    }
}
