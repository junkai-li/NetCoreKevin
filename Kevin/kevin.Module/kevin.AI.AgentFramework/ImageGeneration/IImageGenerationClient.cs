namespace kevin.AI.AgentFramework.ImageGeneration
{
    /// <summary>
    /// 文生图模型配置：由 Application 层从 TAIModels 解析后传入，
    /// 避免 kevin.AI.AgentFramework 反向依赖 kevin.Domain 的 IAIModelsService。
    /// </summary>
    /// <param name="Endpoint">模型 API 基地址（例：https://api.openai.com/v1/）</param>
    /// <param name="ModelName">模型名（例：dall-e-3 / cogview-3-flash / wanx-v1）</param>
    /// <param name="ApiKey">API 密钥，为空时按无鉴权处理（本地部署场景）</param>
    public sealed record ImageGenModelConfig(string Endpoint, string ModelName, string? ApiKey);

    /// <summary>文生图请求参数</summary>
    /// <param name="Prompt">图片描述文本</param>
    /// <param name="Model">模型名</param>
    /// <param name="Size">尺寸，如 1024x1024 / 1792x1024 / 1024x1792</param>
    /// <param name="N">生成张数</param>
    /// <param name="Quality">质量：standard / hd</param>
    /// <param name="ResponseFormat">响应格式：b64_json（推荐） / url</param>
    public sealed record ImageGenerationRequest(
        string Prompt,
        string Model,
        string Size,
        int N,
        string Quality,
        string ResponseFormat);

    /// <summary>单张生成结果</summary>
    /// <param name="Url">URL 响应模式下的图片地址（可能短期失效）</param>
    /// <param name="B64Bytes">b64_json 响应模式下解码后的图片字节</param>
    /// <param name="MimeType">图片 MIME，b64 模式默认 image/png</param>
    /// <param name="RevisedPrompt">模型改写后的 prompt（DALL-E 3 会返回）</param>
    public sealed record GeneratedImage(string? Url, byte[]? B64Bytes, string? MimeType, string? RevisedPrompt);

    /// <summary>文生图整体响应</summary>
    /// <param name="Images">生成的图片列表</param>
    public sealed record ImageGenerationResult(List<GeneratedImage> Images);

    /// <summary>
    /// 文生图客户端抽象。
    /// <para>
    /// 单独抽接口的动机：OpenAI-compat 是本次唯一实现，但后续接入 DashScope 原生 wanx 接口（异步任务模式）、
    /// Stability AI 等厂商时，只需新增实现类替换 DI 注册，不影响上层 AIFunction 与 Service。
    /// </para>
    /// </summary>
    public interface IImageGenerationClient
    {
        /// <summary>
        /// 根据文字描述生成图片
        /// </summary>
        /// <param name="config">目标模型配置（endpoint/key/model）</param>
        /// <param name="request">生成参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>生成结果，至少包含一张图片；失败时抛异常，由上层 AIFunction 转成 ❌ 前缀的友好文案</returns>
        Task<ImageGenerationResult> GenerateAsync(
            ImageGenModelConfig config,
            ImageGenerationRequest request,
            CancellationToken cancellationToken = default);
    }
}
