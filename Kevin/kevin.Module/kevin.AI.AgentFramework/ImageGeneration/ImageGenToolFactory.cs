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

        /// <summary>
        /// IFileStorage 用可选注入：未部署对象存储时文生图仍可工作，只是回落为 base64 内嵌 markdown
        /// （<see cref="ImageGenerationOptions.AutoUploadToOss"/>=false 的行为）
        /// </summary>
        public ImageGenToolFactory(IImageGenerationClient client, IFileStorage? fileStorage = null)
        {
            _client = client;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public AITool BuildGenerateFunction(ImageGenModelConfig config)
        {
            var tool = new ImageGenTool(_client, _fileStorage, config);
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
            private readonly ImageGenModelConfig _config;

            public ImageGenTool(IImageGenerationClient client, IFileStorage? fileStorage, ImageGenModelConfig config)
            {
                _client = client;
                _fileStorage = fileStorage;
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
            /// 若接口直接返回 url（少数不支持 b64 的厂商），透传该 url。
            /// </para>
            /// </summary>
            private async Task<string?> ResolveImageLinkAsync(GeneratedImage img, ImageGenerationOptions options)
            {
                // 情况 1：接口返回了 URL（response_format=url 或 b64 失败回落）
                if (img.B64Bytes == null || img.B64Bytes.Length == 0)
                {
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
            /// b64 → 临时文件 → <see cref="IFileStorage.FileUpload"/> → 稳定 URL。
            /// <para>
            /// IFileStorage 只暴露 localPath 版本，没有 Stream 版本，所以必须落一次磁盘；
            /// 用完立即删除临时文件，避免堆积。
            /// </para>
            /// </summary>
            private string? TryUploadToOss(byte[] bytes, string remotePathPrefix)
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"imagegen_{Guid.NewGuid():N}.png");
                try
                {
                    File.WriteAllBytes(tempPath, bytes);
                    var datePath = string.IsNullOrWhiteSpace(remotePathPrefix)
                        ? "/Files/ImageGen"
                        : remotePathPrefix.TrimEnd('/');
                    datePath += "/" + DateTime.Now.ToString("yyyy/MM/dd");
                    var fileName = $"gen_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.png";
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
