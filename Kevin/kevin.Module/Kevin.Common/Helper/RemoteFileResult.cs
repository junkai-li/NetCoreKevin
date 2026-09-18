using System.IO;

namespace NetCore.Util
{
    /// <summary>
    /// 远程文件下载结果：显式携带 MIME 与文件名，供多模态消息构造使用。
    /// <para>
    /// 独立于 <see cref="FileHelper.GetRemoteFileStreamAsync"/> 的纯 Stream 返回值：
    /// MemoryStream 没有 Name，<c>Microsoft.Extensions.AI.DataContent.LoadFromAsync</c> 只能靠显式 mimeType 参数
    /// 判定图片/音频类型，缺了它就会被判为 <c>application/octet-stream</c> 进而被 OpenAI-compat 客户端拒收。
    /// </para>
    /// </summary>
    /// <param name="Stream">文件内容流（MemoryStream，可 seek，可重复读取）</param>
    /// <param name="MimeType">HTTP Content-Type，兜底后仍为 <c>application/octet-stream</c> 时表示无法识别</param>
    /// <param name="FileName">文件名（从 URL 提取，可能为占位 "file"）</param>
    /// <param name="Length">字节数，用于上层做体积守卫</param>
    public sealed record RemoteFileResult(Stream Stream, string MimeType, string FileName, long Length);
}
