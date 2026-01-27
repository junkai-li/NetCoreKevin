using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Tools
{
    public static class HtmlReader
    {
        public static async Task<string> ExtractTextFromStreamAsync(Stream stream)
        {
            var doc = new HtmlDocument();

            // Detect encoding and load HTML
            using var reader = new StreamReader(stream, Encoding.UTF8, true);
            string content = await reader.ReadToEndAsync();
            doc.LoadHtml(content);

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
                    string text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
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
            "p", "pre", "section", "table", "ul"
        };

        private static bool IsBlockElement(string tagName)
        {
            return Array.IndexOf(BlockElements, tagName) >= 0;
        }
    }
}
