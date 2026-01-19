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
        OllamaEmbedding = 11

    }

    /// <summary>
    /// 模型类型
    /// </summary>
    public enum AIModelType
    {
        Chat = 1,//聊天对话模型
        Embedding = 2,//矢量模型
        Rerank = 4//重排模型
    }
}
