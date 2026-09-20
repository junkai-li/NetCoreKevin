using kevin.AI.AgentFramework.Dto;
using kevin.FileStorage;
using Kevin.log4Net;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
// 与 Microsoft.Extensions.AI.ImageGenerationOptions（IChatClient 图片生成选项）重名，
// 本模块内的 ImageGenerationOptions 特指 kevin.AI.AgentFramework.Dto 下的 appsettings 配置类，用别名消歧
using ImageGenerationOptions = kevin.AI.AgentFramework.Dto.ImageGenerationOptions;

namespace kevin.AI.AgentFramework.ImageGeneration
{
    /// <summary>
    /// <see cref="IImageGenToolFactory"/> 默认实现：把 <see cref="IImageGenerationClient"/> + <see cref="IFileStorage"/>
    /// 通过内部 <see cref="ImageGenTool"/> 实例封装为 AIFunction。
    /// <para>
    /// Scoped 注册：与 <see cref="IFileStorage"/> 生命周期对齐，同一次请求内多次调用工具共享 OSS 客户端。
    /// </para>
    /// </summary>
    public class ImageGenToolFactory : IImageGenToolFactory
    {
        private readonly IImageGenerationClient _client;
        private readonly IFileStorage? _fileStorage;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// IFileStorage 用可选注入：未部署对象存储时文生图仍可工作，只是回落为 base64 内嵌 markdown
        /// （<see cref="ImageGenerationOptions.AutoUploadToOss"/>=false 的行为）。
        /// IHttpClientFactory 用于把厂商只回 URL 的生成结果下载后转存 OSS（CogView 等厂商忽略
        /// response_format=b64_json，只给带过期时间的临时 URL）。
        /// </summary>
        public ImageGenToolFactory(IImageGenerationClient client, IHttpClientFactory httpClientFactory, IFileStorage? fileStorage = null)
        {
            _client = client;
            _fileStorage = fileStorage;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public AITool BuildGenerateFunction(ImageGenModelConfig config)
        {
            var tool = new ImageGenTool(_client, _fileStorage, _httpClientFactory, config);
            return AIFunctionFactory.Create(tool.GenerateImage, new AIFunctionFactoryOptions
            {
                Name = "GenerateImage",
                Description = "根据文字描述生成图片。当用户要求绘画、生成图片、创作视觉内容、画一张/一个 XX 时调用。返回值是 markdown 图片链接（![generated](url)），你必须把它原样复制到最终回复中，让用户看到图片预览；只描述\"已生成\"而不带链接等同任务失败。"
            });
        }

        /// <summary>
        /// 单次工具调用的承载类：捕获模型配置 + 依赖服务，暴露符合 AIFunction 反射规范的实例方法。
        /// <para>
        /// 参数上的 <see cref="DescriptionAttribute"/> 会被 AIFunctionFactory 读取生成 JSON Schema，
        /// 供模型理解参数含义；<c>size</c> 是可选参数，必须有默认值（遵循项目 AIFunction 规范：
        /// 可选性由 HasDefaultValue 决定，不是由 nullability 决定）。
        /// </para>
        /// </summary>
        private sealed class ImageGenTool
        {
            private readonly IImageGenerationClient _client;
            private readonly IFileStorage? _fileStorage;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly ImageGenModelConfig _config;

            public ImageGenTool(IImageGenerationClient client, IFileStorage? fileStorage, IHttpClientFactory httpClientFactory, ImageGenModelConfig config)
            {
                _client = client;
                _fileStorage = fileStorage;
                _httpClientFactory = httpClientFactory;
                _config = config;
            }

            /// <summary>
            /// AIFunction 入口方法。返回 markdown 字符串：成功时 <c>![generated](url)</c>，失败时 <c>❌ ...</c> 前缀，
            /// 与项目其他工具（SaveFileContent 等）保持一致的错误约定，模型能理解并复述给用户。
            /// </summary>
            [Description("根据文字描述生成图片，返回 markdown 图片链接。")]
            public async Task<string> GenerateImage(
                [Description("图片的详细文字描述，越具体越好（主体、风格、色调、构图等）")][Required] string prompt,
                [Description("图片尺寸，可选值：1024x1024（正方形）/ 1792x1024（横版）/ 1024x1792（竖版）")] string size = "1024x1024")
            {
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    return "❌ 生成失败: prompt 不能为空";
                }
                try
                {
                    var options = ImageGenerationOptions.Current;
                    var effectiveSize = string.IsNullOrWhiteSpace(size) ? options.DefaultSize : size;
                    var result = await _client.GenerateAsync(
                        _config,
                        new ImageGenerationRequest(prompt, _config.ModelName, effectiveSize, options.DefaultN, options.DefaultQuality, "b64_json"));

                    if (result.Images.Count == 0)
                    {
                        return "❌ 生成失败: 接口未返回图片";
                    }

                    var markdowns = new List<string>();
                    foreach (var img in result.Images)
                    {
                        var link = await ResolveImageLinkAsync(img, options);
                        if (string.IsNullOrEmpty(link))
                        {
                            markdowns.Add("❌ 单张图片落地失败");
                            continue;
                        }
                        markdowns.Add($"![generated]({link})");
                    }

                    var revised = result.Images.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.RevisedPrompt))?.RevisedPrompt;
                    var suffix = string.IsNullOrWhiteSpace(revised) ? "" : $"\n\n图片主题：{revised}";
                    // 工具返回值里再明确一次透传指令，形成 systemPrompt + Description + 返回值三重强化：
                    // 部分模型（尤其小参数模型）对 system prompt 遵从度不高，但看到工具结果里的显式指令通常会照做，
                    // 避免出现"我已经为你生成了一张图片"这种没链接的空回复让用户看不到图。
                    const string instruction = "【请把下面这行 markdown 图片链接原样复制到你的最终回复中，让用户看到图片预览；不要改动 URL、不要包在代码块里、不要只描述\"已生成\"而不带链接】\n";
                    return instruction + string.Join("\n\n", markdowns) + suffix;
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("文生图工具调用失败:", ex);
                    return $"❌ 生成失败: {ex.Message}";
                }
            }

            /// <summary>
            /// 把单张生成图片落地为可访问 URL：
            /// <para>
            /// 优先级：b64 字节 → OSS 上传（若启用且 IFileStorage 可用） → 内嵌 data URL；
            /// 接口只回 url 时（CogView 等厂商忽略 response_format=b64_json），同样下载字节后转存 OSS，
            /// 因为厂商 URL 是带过期时间的临时地址，直接透传会让几小时后的聊天记录图片裂图；
            /// 下载/转存失败或 OSS 未启用时才回落透传原 URL。
            /// </para>
            /// </summary>
            private async Task<string?> ResolveImageLinkAsync(GeneratedImage img, ImageGenerationOptions options)
            {
                // 情况 1：接口只返回了 URL（没有 b64 字节）：下载后转存 OSS 拿稳定链接
                if (img.B64Bytes == null || img.B64Bytes.Length == 0)
                {
                    if (string.IsNullOrEmpty(img.Url)) return null;

                    if (options.AutoUploadToOss && _fileStorage != null)
                    {
                        var bytes = await DownloadImageBytesAsync(img.Url);
                        if (bytes is { Length: > 0 })
                        {
                            var ossUrl = TryUploadToOss(bytes, options.OssRemotePath, GuessExtensionFromUrl(img.Url, img.MimeType));
                            if (!string.IsNullOrEmpty(ossUrl)) return ossUrl;
                        }
                        // 下载或转存失败不判死刑：透传原 URL 让用户至少现在能看到图（链接过期是后话）
                        LogHelper.logger.Warn($"文生图 URL 转存 OSS 失败，回落透传原 URL: {img.Url}");
                    }
                    return img.Url;
                }

                // 情况 2：有 b64 字节 + 启用 OSS 上传 + IFileStorage 已注入
                if (options.AutoUploadToOss && _fileStorage != null)
                {
                    var ossUrl = TryUploadToOss(img.B64Bytes, options.OssRemotePath);
                    if (!string.IsNullOrEmpty(ossUrl)) return ossUrl;
                    // OSS 失败时不直接抛异常，回落到内嵌 data URL，保证用户至少能看到图
                    LogHelper.logger.Warn($"文生图 OSS 上传失败，回落为内嵌 data URL");
                }

                // 情况 3：内嵌 base64 data URL（前端 markdown 渲染器可直接展示）
                var mime = string.IsNullOrWhiteSpace(img.MimeType) ? "image/png" : img.MimeType;
                return $"data:{mime};base64,{Convert.ToBase64String(img.B64Bytes)}";
            }

            /// <summary>
            /// 下载厂商临时图片 URL 的字节。失败返回 null 由调用方回落透传原 URL，不中断整个工具调用。
            /// </summary>
            private async Task<byte[]?> DownloadImageBytesAsync(string url)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient(nameof(ImageGenToolFactory));
                    // 下载的是已生成好的静态图，正常亚秒级；但跨云拉取可能慢，复用生成接口的超时配置兜底
                    client.Timeout = TimeSpan.FromSeconds(ImageGenerationOptions.Current.HttpTimeoutSeconds);
                    return await client.GetByteArrayAsync(url);
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error($"下载文生图返回 URL 失败: {url}", ex);
                    return null;
                }
            }

            /// <summary>
            /// 从图片 URL / MIME 猜文件扩展名：CogView 等常返回 .jpg，不能固定按 .png 存；
            /// 认不出来时默认 .png（与 b64 路径 MimeType=image/png 的既有约定一致）。
            /// </summary>
            private static string GuessExtensionFromUrl(string url, string? mimeType)
            {
                var ext = Path.GetExtension(url.Split('?')[0]).ToLowerInvariant();
                if (ext is ".png" or ".jpg" or ".jpeg" or ".webp") return ext;
                return mimeType?.ToLowerInvariant() switch
                {
                    "image/jpeg" => ".jpg",
                    "image/webp" => ".webp",
                    _ => ".png",
                };
            }

            /// <summary>
            /// 字节 → 临时文件 → <see cref="IFileStorage.FileUpload"/> → 稳定 URL。
            /// <para>
            /// IFileStorage 只暴露 localPath 版本，没有 Stream 版本，所以必须落一次磁盘；
            /// 用完立即删除临时文件，避免堆积。
            /// </para>
            /// </summary>
            private string? TryUploadToOss(byte[] bytes, string remotePathPrefix, string extension = ".png")
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"imagegen_{Guid.NewGuid():N}{extension}");
                try
                {
                    File.WriteAllBytes(tempPath, bytes);
                    var datePath = string.IsNullOrWhiteSpace(remotePathPrefix)
                        ? "/Files/ImageGen"
                        : remotePathPrefix.TrimEnd('/');
                    datePath += "/" + DateTime.Now.ToString("yyyy/MM/dd");
                    var fileName = $"gen_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
                    var (success, url) = _fileStorage!.FileUpload(tempPath, datePath, fileName);
                    return success ? url : null;
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("文生图 OSS 上传异常:", ex);
                    return null;
                }
                finally
                {
                    try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { /* 临时文件删不掉不影响主流程 */ }
                }
            }
        }
    }
}
