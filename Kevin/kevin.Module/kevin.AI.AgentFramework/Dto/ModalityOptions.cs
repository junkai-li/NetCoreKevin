using Kevin.Common.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// 多模态输入守卫配置（对应 appsettings 的 Modality 节，可不配置，全部走此处默认值）。
    /// <para>
    /// 与 <see cref="AIChatStorageSetting"/> 同源问题：图片/音频体积过大同样会撑爆 MySQL 单行 INSERT
    /// 或触发上游模型的 payload 上限；这里在 <c>ModalityContentBuilder</c> 下载完成后立即拦截，
    /// 避免让不合规文件走完整个消息构造流程再失败。
    /// </para>
    /// <para>
    /// 填 0 或负数视为"未配置"并沿用默认值；<c>AllowedAudioMimeTypes</c> 为空时视为不限制 MIME。
    /// 修改 appsettings 后随配置热重载生效，无需重启。
    /// </para>
    /// </summary>
    public class ModalityOptions
    {
        /// <summary>配置节名称</summary>
        private const string SectionKey = "Modality";

        /// <summary>单个音频文件字节上限（默认 20MB，覆盖大部分语音提问场景）</summary>
        public long MaxAudioFileBytes { get; set; } = 20 * 1024 * 1024;

        /// <summary>单个图片文件字节上限（默认 10MB，OpenAI vision 单图上限约 20MB，这里留一半余量）</summary>
        public long MaxImageFileBytes { get; set; } = 10 * 1024 * 1024;

        /// <summary>允许的音频 MIME 白名单，空列表视为不限制</summary>
        public List<string> AllowedAudioMimeTypes { get; set; } = new()
        {
            "audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg",
            "audio/flac", "audio/aac", "audio/opus", "audio/amr"
        };

        /// <summary>
        /// 单条消息内允许携带的媒体附件（图片+音频）总数上限，防止一次上传几十个文件把请求打爆
        /// </summary>
        public int MaxMediaAttachmentsPerMessage { get; set; } = 10;

        private static readonly object _lock = new();
        private static ModalityOptions? _cached;
        private static IChangeToken? _cachedToken;

        /// <summary>
        /// 当前生效的配置：进程内缓存，appsettings 变更后自动重新读取（与 AIChatStorageSetting 同一套缓存策略）
        /// </summary>
        public static ModalityOptions Current
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

        /// <summary>
        /// 读取配置节；配置未初始化（如单元测试）或节缺失时返回默认值，守卫本身不能反过来让请求出错
        /// </summary>
        private static ModalityOptions Load()
        {
            var setting = new ModalityOptions();
            try
            {
                var section = ConfigHelper.Configuration?.GetSection(SectionKey);
                if (section == null) return setting;
                setting.MaxAudioFileBytes = ReadLong(section[nameof(MaxAudioFileBytes)], setting.MaxAudioFileBytes);
                setting.MaxImageFileBytes = ReadLong(section[nameof(MaxImageFileBytes)], setting.MaxImageFileBytes);
                setting.MaxMediaAttachmentsPerMessage = ReadInt(section[nameof(MaxMediaAttachmentsPerMessage)], setting.MaxMediaAttachmentsPerMessage);
                var mimeSection = section.GetSection(nameof(AllowedAudioMimeTypes));
                if (mimeSection.Exists())
                {
                    var list = mimeSection.Get<List<string>>();
                    if (list != null) setting.AllowedAudioMimeTypes = list;
                }
            }
            catch
            {
                return new ModalityOptions();
            }
            return setting;
        }

        private static long ReadLong(string? value, long defaultValue)
            => long.TryParse(value, out var result) && result > 0 ? result : defaultValue;

        private static int ReadInt(string? value, int defaultValue)
            => int.TryParse(value, out var result) && result > 0 ? result : defaultValue;
    }
}
