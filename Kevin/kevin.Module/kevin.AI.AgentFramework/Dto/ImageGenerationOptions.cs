using Kevin.Common.Helper;
using Microsoft.Extensions.Primitives;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// 文生图默认参数配置（对应 appsettings 的 ImageGeneration 节，可不配置，全部走此处默认值）。
    /// <para>
    /// 与 <see cref="AIChatStorageSetting"/> / <see cref="ModalityOptions"/> 同源模式：静态 <see cref="Current"/> + ReloadToken 缓存，
    /// 避免每次工具调用都去解析配置节。
    /// </para>
    /// </summary>
    public class ImageGenerationOptions
    {
        /// <summary>配置节名称</summary>
        private const string SectionKey = "ImageGeneration";

        /// <summary>默认图片尺寸（模型未指定 size 参数时使用）</summary>
        public string DefaultSize { get; set; } = "1024x1024";

        /// <summary>默认图片质量：standard / hd（DALL-E 3 支持，其他厂商可能忽略）</summary>
        public string DefaultQuality { get; set; } = "standard";

        /// <summary>默认生成张数</summary>
        public int DefaultN { get; set; } = 1;

        /// <summary>HTTP 请求超时（秒），文生图通常比对话慢，默认 120s</summary>
        public int HttpTimeoutSeconds { get; set; } = 120;

        /// <summary>
        /// 是否自动上传生成结果到 OSS。
        /// <para>
        /// true：b64 → 临时文件 → <c>IFileStorage.FileUpload</c> → 返回稳定 URL（推荐，DALL-E 官方 URL 仅 1h 有效）；
        /// false：直接返回 <c>data:image/png;base64,...</c> 内嵌 markdown，前端浏览器可渲染但 DB 会膨胀，
        /// 由 <see cref="AIChatStorageSetting.AnswerContentMaxLength"/> 兜底截断。
        /// </para>
        /// </summary>
        public bool AutoUploadToOss { get; set; } = true;

        /// <summary>OSS 存放路径前缀（AutoUploadToOss=true 时生效）</summary>
        public string OssRemotePath { get; set; } = "/Files/ImageGen";

        private static readonly object _lock = new();
        private static ImageGenerationOptions? _cached;
        private static IChangeToken? _cachedToken;

        /// <summary>当前生效的配置</summary>
        public static ImageGenerationOptions Current
        {
            get
            {
                var token = ConfigHelper.Configuration?.GetReloadToken();
                var cached = _cached;
                if (cached != null && ReferenceEquals(_cachedToken, token)) return cached;
                lock (_lock)
                {
                    token = ConfigHelper.Configuration?.GetReloadToken();
                    cached = _cached;
                    if (cached != null && ReferenceEquals(_cachedToken, token)) return cached;
                    _cached = Load();
                    _cachedToken = token;
                    return _cached;
                }
            }
        }

        private static ImageGenerationOptions Load()
        {
            var setting = new ImageGenerationOptions();
            try
            {
                var section = ConfigHelper.Configuration?.GetSection(SectionKey);
                if (section == null) return setting;
                setting.DefaultSize = ReadString(section[nameof(DefaultSize)], setting.DefaultSize);
                setting.DefaultQuality = ReadString(section[nameof(DefaultQuality)], setting.DefaultQuality);
                setting.DefaultN = ReadInt(section[nameof(DefaultN)], setting.DefaultN, minValue: 1);
                setting.HttpTimeoutSeconds = ReadInt(section[nameof(HttpTimeoutSeconds)], setting.HttpTimeoutSeconds, minValue: 10);
                setting.OssRemotePath = ReadString(section[nameof(OssRemotePath)], setting.OssRemotePath);
                if (bool.TryParse(section[nameof(AutoUploadToOss)], out var autoUpload))
                {
                    setting.AutoUploadToOss = autoUpload;
                }
            }
            catch
            {
                return new ImageGenerationOptions();
            }
            return setting;
        }

        private static string ReadString(string? value, string defaultValue)
            => string.IsNullOrWhiteSpace(value) ? defaultValue : value!;

        private static int ReadInt(string? value, int defaultValue, int minValue)
            => int.TryParse(value, out var result) && result >= minValue ? result : defaultValue;
    }
}
