namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// 消息模态类型：标识一段内容属于哪种感官通道。
    /// <para>
    /// 由 kevin.AI.AgentFramework/Modality 抽象层用于分类构造 <c>Microsoft.Extensions.AI.AIContent</c>：
    /// Text/Document → <c>TextContent</c>，Image/Audio → <c>DataContent</c>（携带对应 MIME），
    /// Video 预留位（本次未实现），避免后续加视频模态时再动接口签名。
    /// </para>
    /// </summary>
    public enum ModalityKind
    {
        /// <summary>纯文本内容</summary>
        Text = 1,
        /// <summary>图片：png/jpg/webp 等，走 DataContent + image/* MIME</summary>
        Image = 2,
        /// <summary>音频：mp3/wav/m4a 等，走 DataContent + audio/* MIME，由 OpenAI-compat 客户端映射为 input_audio</summary>
        Audio = 3,
        /// <summary>文档：excel/pdf/word/html/markdown/text，先解析为纯文本再作为 TextContent 送入模型</summary>
        Document = 4,
        /// <summary>视频：预留位，本次未实现</summary>
        Video = 5
    }
}
