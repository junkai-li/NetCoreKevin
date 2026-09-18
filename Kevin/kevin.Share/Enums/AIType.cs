using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// AI类型
    /// </summary>
    public enum AIType
    {
        //兼容大部分AI平台
        [Display(Name = "Open AI")]
        OpenAI = 1,
        [Display(Name = "Azure Open AI")]
        AzureOpenAI = 2,
        [Display(Name = "智谱AI")]
        ZhiPuAI = 3,
        [Display(Name = "Bge Embedding")]
        BgeEmbedding = 7,
        [Display(Name = "Bge Rerank")]
        BgeRerank = 8,
        [Display(Name = "Ollama")]
        Ollama = 10,
        [Display(Name = "OllamaEmbedding")]
        OllamaEmbedding = 11,
        [Display(Name = "Ali Rerank")]
        AliRerank = 12, 
    }

    /// <summary>
    /// 模型类型
    /// <para>
    /// 采用位标记语义：Chat/Embedding/Rerank 是"主用途"互斥值；
    /// AudioUnderstanding/ImageGeneration 是"能力叠加"标记，可与主用途按位或组合。
    /// 例如"支持音频输入的对话模型" = Chat | AudioUnderstanding = 17。
    /// </para>
    /// </summary>
    [Flags]
    public enum AIModelType
    {
        Chat = 1,//聊天对话模型
        Embedding = 2,//矢量模型
        Rerank = 4,//重排模型
        /// <summary>
        /// 文生图能力（DALL-E / CogView / 通义万相 OpenAI-compat），
        /// 通常作为独立模型条目存在，不与 Chat 组合
        /// </summary>
        ImageGeneration = 8,
        /// <summary>
        /// 音频理解能力（gpt-4o-audio / qwen2-audio / glm-4-voice），
        /// 一般与 Chat 组合使用（Chat | AudioUnderstanding）标记"能听音频的对话模型"
        /// </summary>
        AudioUnderstanding = 16
    }
}
