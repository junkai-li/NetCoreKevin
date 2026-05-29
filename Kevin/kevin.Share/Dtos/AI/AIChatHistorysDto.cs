using kevin.Domain.Share.Dtos.Bases;
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
    }
}
