using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace Kevin.RAG.Tools
{
    public static class HtmlReader
    {
        public static async Task<string> ExtractTextFromStreamAsync(Stream stream)
        {
            if (stream == null) return string.Empty;

            var doc = new HtmlDocument();

            // Ensure we read from stream start when possible
            if (stream.CanSeek)
            {
                try { stream.Position = 0; } catch { }
            }

            // Read stream without closing underlying stream
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            string content = await reader.ReadToEndAsync();

            // If content empty, try loading directly from stream copy (handles some encodings/BOM cases)
            if (string.IsNullOrWhiteSpace(content))
            {
                if (stream.CanSeek)
                {
                    try { stream.Position = 0; } catch { }
                }
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                doc.Load(ms);
            }
            else
            {
                doc.LoadHtml(content);
            }

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
    }
}
