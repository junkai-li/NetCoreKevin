using System.Text.RegularExpressions;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 模型可控文本的安全护栏（纯函数，不依赖 Scoped 服务）。
    /// Shell / Python / 技能脚本三条执行链路共用这一份实现：
    /// 此前 ShellToolsService 修掉的“字面量匹配被语法变体绕过”没有同步到 Python 侧，
    /// 两条工具对同一份命令给出相反的放行结论。
    /// </summary>
    public static class CommandGuardrails
    {
        /// <summary>受限配置文件清单</summary>
        public static readonly string[] RestrictedFiles = [
            "appsettings.json",
            "appsettings.development.json",
            "appsettings.test.json"
        ];

        /// <summary>
        /// 危险命令黑名单。
        /// 只适用于 Shell 形态的文本：这些模式是 cmd/bash/pwsh 语法，
        /// 拿去匹配 Python 代码会误伤（如 "format " 命中 str.format 调用）。
        /// </summary>
        public static readonly string[] DangerousPatterns = [
            "rm -rf /", "rm -rf /*",       // 删除根目录
            "sudo ",                        // 提权
            "shutdown", "reboot",           // 系统操作
            "> /dev/",                      // 设备写入
            ":(){ :|:& };:",                // Fork bomb
            "mkfs.",                        // 格式化
            "dd if=",                       // 磁盘覆写
            "format ",                      // Windows 格式化
            "del /f /s /q",                 // Windows 递归删除
        ];

        public const string DangerousCommandBlockedMessage = "❌ 安全拦截：检测到危险命令，已阻止执行。";
        public const string RestrictedFileBlockedMessage = "❌ 安全拦截：禁止访问配置文件（appsettings.json 等）。";

        public static string UnauthorizedDomainMessage(string url) => $"❌ 授权拦截：URL '{url}' 不在授权域名单中。";

        // cmd / bash 用于分词、引号包裹、转义、通配与路径拼接的噪声字符。
        // 剔除后，下面这些写法会收敛到同一形态，从而不再能绕过字面量匹配：
        //   appsettings*.json / APPSET~1.JSON / "appset"tings.json / set X=appsettings && type %X%.json
        private static readonly Regex ShellNoiseChars = new(@"[\s""'`^%*?~\\/.:=,()&|;<>-]", RegexOptions.Compiled);

        // 归一化后的文件名特征前缀（取前 6 字符，正好覆盖 8.3 短名的六位截断）
        private static readonly string[] RestrictedStems = RestrictedFiles
            .Select(file => ShellNoiseChars.Replace(file, ""))
            .Select(normalized => normalized.Length >= 6 ? normalized[..6] : normalized)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // 用于从文本中提取URL的正则表达式
        private static readonly Regex UrlRegex = new Regex(@"https?://[\w\-._~:/?#\[\]@!$&'()*+,;=%]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // 上面的字符类允许 ' ( ) , ; 等，但真实 URL 极少以它们结尾：
        // Python 代码里的 requests.get('https://x/y').text 会连同 “').text)” 一起被当成 URL，
        // 既污染拦截提示，又给“是否命中授权域名”掺进无关字符。尾部一律裁掉，
        // 只会把地址缩短（判定的主机名/路径不会因此变宽），不会造成漏拦。
        private const string TrailingUrlChars = "'\"(),;:.!?~";

        /// <summary>
        /// 检查文本是否指向受限配置文件。
        /// 先按原样匹配，再对剔除噪声字符后的文本做文件名前缀匹配，
        /// 以覆盖通配符、8.3 短名、引号拆分、变量拼接等绕过写法。
        /// </summary>
        public static bool ContainsRestrictedFile(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (RestrictedFiles.Any(file => text.Contains(file, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var normalized = ShellNoiseChars.Replace(text, "");
            return RestrictedStems.Any(stem => normalized.Contains(stem, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 检查 Shell 形态文本是否命中危险命令黑名单
        /// </summary>
        public static bool ContainsDangerousPattern(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return DangerousPatterns.Any(d => text.Contains(d, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 通配条目：等同于不启用域名护栏
        /// </summary>
        private const string WildcardEntry = "*";

        /// <summary>
        /// 解析逗号分隔的授权域名配置（智能体表里的 AuthorizedDomains 就是这个形态）。
        /// "*" / 空 / 全空白 都返回空列表，含义是“不启用该护栏”。
        /// 通配项与具体域名混写（"*,foo.com"）时丢弃通配项，只按域名判：
        /// 把混写解释成“全放行”只会在手滑时把整道护栏关掉。
        /// </summary>
        public static List<string> ParseDomainEntries(string? commaSeparatedDomains)
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedDomains)
                || commaSeparatedDomains.Trim() == WildcardEntry)
            {
                return new List<string>();
            }
            return commaSeparatedDomains
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(entry => !string.IsNullOrWhiteSpace(entry) && entry != WildcardEntry)
                .ToList();
        }

        /// <summary>
        /// 判断单个完整 URL 是否落在授权范围内（http/https 工具直接拿着 url 参数走这一条）。
        /// 名单为 null / 空 / 只含通配项时放行，等同于不启用该护栏。
        /// 与 <see cref="FindUnauthorizedUrl"/> 共用同一套主机名边界判定，避免两条链路对同一个 URL 给出相反结论。
        /// </summary>
        public static bool IsUrlAuthorized(string? url, IReadOnlyCollection<string>? authorizedDomains)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return true;
            }
            var entries = ResolveEntries(authorizedDomains);
            if (entries is null)
            {
                return true;
            }
            return Authorize(TrimUrl(url), entries);
        }

        /// <summary>
        /// 找出文本中第一个不在授权域名白名单内的 URL；全部放行时返回 null。
        /// 白名单为空（或含空白项）等同于不启用该护栏。
        /// </summary>
        public static string? FindUnauthorizedUrl(string text, IReadOnlyCollection<string>? authorizedDomains)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var allowedPrefixes = ResolveEntries(authorizedDomains);
            if (allowedPrefixes is null)
            {
                return null;
            }

            foreach (Match match in UrlRegex.Matches(text))
            {
                var url = TrimUrl(match.Value);
                if (url.Length == 0)
                {
                    continue;
                }
                if (!Authorize(url, allowedPrefixes))
                {
                    return url;
                }
            }
            return null;
        }

        /// <summary>
        /// 过滤名单：剔除空白项与通配项；没有有效条目时返回 null，表示不启用该护栏
        /// </summary>
        private static List<string>? ResolveEntries(IReadOnlyCollection<string>? authorizedDomains)
        {
            var entries = authorizedDomains
                ?.Where(domain => !string.IsNullOrWhiteSpace(domain) && domain.Trim() != WildcardEntry)
                .ToList();
            return entries is null || entries.Count == 0 ? null : entries;
        }

        private static string TrimUrl(string raw)
        {
            // 代码里的字符串字面量常与 URL 紧贴（requests.get('https://x/y').text），
            // 引号一定是 URL 的边界：从第一个引号处截断，否则 “').text)” 会被当成地址的一部分
            var quoteIndex = raw.IndexOf('\'', StringComparison.Ordinal);
            var doubleQuoteIndex = raw.IndexOf('"', StringComparison.Ordinal);
            if (doubleQuoteIndex >= 0 && (quoteIndex < 0 || doubleQuoteIndex < quoteIndex))
            {
                quoteIndex = doubleQuoteIndex;
            }
            if (quoteIndex > "https://".Length)
            {
                raw = raw[..quoteIndex];
            }
            var end = raw.Length;
            while (end > 0 && TrailingUrlChars.IndexOf(raw[end - 1]) >= 0)
            {
                end--;
            }
            return raw[..end];
        }

        /// <summary>
        /// URL 是否落在授权范围内。
        /// 旧实现是 url.Contains(entry)：只要串里出现过授权域名就放行，
        /// https://evil.com/?to=example.com 与 https://example.com.evil.com/ 都能过，白名单形同虚设。
        /// 现在按主机名边界比对：条目等于主机名，或是其父域（example.com 放行 api.example.com，不放行 evilexample.com）。
        /// URL 解析不出主机名时退回旧的包含式匹配，避开把残缺 URL 的正常用法卡死。
        /// </summary>
        private static bool Authorize(string url, List<string> allowedEntries)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed) || string.IsNullOrEmpty(parsed.Host))
            {
                // 相对 URL 或残缺地址：旧实现也是包含式匹配，沿用，不然会把原来能放行的调用卡死
                return allowedEntries.Any(entry => url.Contains(entry, StringComparison.OrdinalIgnoreCase));
            }
            foreach (var entry in allowedEntries)
            {
                var (host, pathPrefix) = SplitEntry(entry);
                if (host.Length == 0)
                {
                    // 条目写成了纯路径（如 /api/x）这种非域名形态：按旧语义包含式判定，不比旧实现更严
                    if (url.Contains(entry, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    continue;
                }
                // Authority 参与比对：条目写成 example.com:8080 这种带端口的形式时仍能命中
                if (!MatchesHost(parsed.Host, host) && !MatchesHost(parsed.Authority, host))
                {
                    continue;
                }
                // 条目写了路径（如 https://api.example.com/v1）时路径也限在范围内，不比旧实现更宽
                if (pathPrefix.Length > 0
                    && !parsed.AbsolutePath.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        private static bool MatchesHost(string candidate, string host) =>
            string.Equals(candidate, host, StringComparison.OrdinalIgnoreCase)
            || candidate.EndsWith("." + host, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 拆解白名单条目：去掉 scheme 后的主机名部分 + 可选的路径前缀（结尾点与空路径归零长度）
        /// </summary>
        private static (string Host, string PathPrefix) SplitEntry(string entry)
        {
            var rest = entry.Trim();
            var schemeIndex = rest.IndexOf("://", StringComparison.Ordinal);
            if (schemeIndex >= 0)
            {
                rest = rest[(schemeIndex + 3)..];
            }
            var cut = rest.IndexOfAny(new[] { '/', '?', '#' });
            if (cut < 0)
            {
                return (rest.TrimEnd('.'), string.Empty);
            }
            var host = rest[..cut].TrimEnd('.');
            // 查询与锚点（? #）不属于路径，只留 /xxx 部分
            var tail = rest[cut..];
            var queryIndex = tail.IndexOfAny(new[] { '?', '#' });
            if (queryIndex >= 0)
            {
                tail = tail[..queryIndex];
            }
            return (host, tail == "/" ? string.Empty : tail);
        }
    }
}
