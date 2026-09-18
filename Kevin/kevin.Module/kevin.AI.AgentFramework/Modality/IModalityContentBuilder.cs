namespace kevin.AI.AgentFramework.Modality
{
    /// <summary>
    /// 多模态内容构造器：把一个远程文件 URL 转成 <c>Microsoft.Extensions.AI.AIContent</c> 或纯文本片段。
    /// <para>
    /// 抽离出来的动机：<c>AIChatHistorysService.AIFileUrlsHandle</c> 原本直接依赖
    /// <c>FileHelper.DetermineFileType</c> + 5 个 Reader 类 + <c>DataContent.LoadFromAsync</c>，
    /// 加音频/视频模态需要动 Service 主流程；把"文件 → AIContent"这层封成独立接口后，
    /// Service 只管 await 结果、按 <see cref="ModalityContentResult.Kind"/> 分桶归集，
    /// 新增模态只在实现类里加分支即可，不动业务代码。
    /// </para>
    /// <para>
    /// 实现类必须保证：
    /// 1. 完全异步（内部不得出现 <c>.Result</c> / <c>.Wait()</c>），支持上层 <c>Task.WhenAll</c> 并发处理多文件；
    /// 2. 单个文件处理失败不抛异常，改为返回带 <see cref="ModalityContentResult.Error"/> 的结果对象，
    ///    避免一个坏文件把整轮对话打挂（历史行为：AIFileUrlsHandle 内部 try/catch 单文件错误）。
    /// </para>
    /// </summary>
    public interface IModalityContentBuilder
    {
        /// <summary>
        /// 处理单个远程文件
        /// </summary>
        /// <param name="fileUrl">远程文件 URL（OSS/MinIO 直链）</param>
        /// <param name="fileName">文件名，为空时从 URL 提取；扩展名决定分类</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分类结果；<see cref="ModalityContentResult.IsUsable"/> 为 false 时表示该文件不可用</returns>
        Task<ModalityContentResult> BuildAsync(string fileUrl, string? fileName, CancellationToken cancellationToken = default);
    }
}
