using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
namespace Kevin.Common.Helper.FileHandleTools
{
    public static class HtmlReader
    {
        public static async Task<string> ExtractTextFromStreamAsync(Stream stream)
        {
            if (stream == null) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(await ReadHtmlTextFromStreamAsync(stream));

            // Remove scripts and styles
            var scripts = doc.DocumentNode.SelectNodes("//script");
            if (scripts != null)
            {
                foreach (var script in scripts)
                {
                    script.Remove();
                }
            }

            var styles = doc.DocumentNode.SelectNodes("//style");
            if (styles != null)
            {
                foreach (var style in styles)
                {
                    style.Remove();
                }
            }

            // Get text with basic formatting preservation
            var sb = new StringBuilder();
            ProcessNode(doc.DocumentNode, sb);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 从HTML流中提取 <img> 图片源，供多模态模型下载后消费。
        /// <para>
        /// data URI 与 http(s) 绝对地址原样返回；协议相对（//host/path）与相对路径结合 baseUrl
        /// （HTML 文件本身的 URL）解析成绝对地址；baseUrl 缺失导致无法解析的相对路径跳过。
        /// </para>
        /// </summary>
        /// <param name="stream">HTML文件流</param>
        /// <param name="baseUrl">HTML 文件本身的 URL，用于解析相对路径</param>
        /// <returns>图片源列表（data URI 或绝对 URL）</returns>
        public static async Task<List<string>> ExtractImageSourcesAsync(Stream stream, string baseUrl = null)
        {
            var result = new List<string>();
            if (stream == null) return result;

            var doc = new HtmlDocument();
            doc.LoadHtml(await ReadHtmlTextFromStreamAsync(stream));

            var imgs = doc.DocumentNode.SelectNodes("//img");
            if (imgs == null) return result;

            Uri baseUri = null;
            if (!string.IsNullOrWhiteSpace(baseUrl))
                Uri.TryCreate(baseUrl, UriKind.Absolute, out baseUri);

            foreach (var img in imgs)
            {
                var src = img.GetAttributeValue("src", null);
                if (string.IsNullOrWhiteSpace(src)) continue;
                src = HtmlEntity.DeEntitize(src).Trim();
                if (src.Length == 0) continue;

                if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(src);
                    continue;
                }

                if (Uri.TryCreate(src, UriKind.Absolute, out var absolute)
                    && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
                {
                    result.Add(absolute.ToString());
                    continue;
                }

                // 协议相对地址：//host/path，补上 HTML 自身地址的 scheme
                if (src.StartsWith("//") && baseUri != null)
                {
                    result.Add(baseUri.Scheme + ":" + src);
                    continue;
                }

                // 相对路径：基于 HTML 自身地址解析
                if (baseUri != null && Uri.TryCreate(baseUri, src, out var combined))
                {
                    result.Add(combined.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 从流中读出 HTML 文本（ExtractTextFromStreamAsync / ExtractImageSourcesAsync 共用）
        /// </summary>
        private static async Task<string> ReadHtmlTextFromStreamAsync(Stream stream)
        {
            if (stream.CanSeek && stream.Length > 0)
            {
                stream.Position = 0;
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        public static async Task<string> ExtractTextFromTextAsync(string text)
        {
            return await Task.Run(() =>
            {
                var doc = new HtmlDocument();
                // Detect encoding and load HTML  
                doc.LoadHtml(text);

                // Remove scripts and styles
                var scripts = doc.DocumentNode.SelectNodes("//script");
                if (scripts != null)
                {
                    foreach (var script in scripts)
                    {
                        script.Remove();
                    }
                }

                var styles = doc.DocumentNode.SelectNodes("//style");
                if (styles != null)
                {
                    foreach (var style in styles)
                    {
                        style.Remove();
                    }
                }

                // Get text with basic formatting preservation
                var sb = new StringBuilder();
                ProcessNode(doc.DocumentNode, sb);
                return sb.ToString().Trim();
            });
        }

        private static void ProcessNode(HtmlNode node, StringBuilder sb)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Document:
                    // Document 节点是根节点，递归处理其子节点
                    foreach (var child in node.ChildNodes)
                    {
                        ProcessNode(child, sb);
                    }
                    break;

                case HtmlNodeType.Text:
                    string text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        // normalize whitespace
                        text = Regex.Replace(text, "\\s+", " ");
                        sb.Append(text).Append(' ');
                    }
                    break;

                case HtmlNodeType.Element:
                    // Add line breaks for block-level elements
                    bool isBlockElement = IsBlockElement(node.Name.ToLower());
                    if (isBlockElement && sb.Length > 0 && !char.IsWhiteSpace(sb[sb.Length - 1]))
                    {
                        sb.AppendLine();
                    }

                    // Process children
                    foreach (var child in node.ChildNodes)
                    {
                        ProcessNode(child, sb);
                    }

                    // Add line breaks after closing block elements
                    if (isBlockElement && sb.Length > 0 && !char.IsWhiteSpace(sb[sb.Length - 1]))
                    {
                        sb.AppendLine();
                    }
                    break;

                case HtmlNodeType.Comment:
                    // 忽略注释节点
                    break;
            }
        }

        private static readonly string[] BlockElements = {
            "address", "article", "aside", "blockquote", "details", "dialog",
            "dd", "div", "dl", "dt", "fieldset", "figcaption", "figure",
            "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6",
            "header", "hgroup", "hr", "li", "main", "nav", "ol",
            "p", "pre", "section", "table", "ul", "br"
        };

        private static bool IsBlockElement(string tagName)
        {
            return Array.IndexOf(BlockElements, tagName) >= 0;
        }

        public static async Task<string> ExtractTextFromUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            var (stream, _, _) = await RemoteDocumentLoader.Default.DownloadWithMetadataAsync(url, cancellationToken);
            using (stream)
            {
                return await ExtractTextFromStreamAsync(stream);
            }
        }
    }
}
