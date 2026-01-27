using System;
using System.IO;
using System.Text;
using System.Text.Json;
namespace Kevin.RAG;
public class TextStreamReader
{
    /// <summary>
    /// 从文件流中自动检测编码并读取纯文本内容
    /// </summary>
    public static string ReadTextFromStream(Stream stream)
    {
        if (stream == null || !stream.CanRead)
            throw new ArgumentException("无效的文件流");

        // 优先检测BOM以确定编码
        var buffer = new byte[4];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        stream.Position = 0; // 重置流位置以便后续读取

        Encoding encoding;
        if (bytesRead >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            encoding = Encoding.UTF8; // UTF-8 with BOM
        else if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
            encoding = Encoding.Unicode; // UTF-16 LE
        else if (bytesRead >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF)
            encoding = Encoding.BigEndianUnicode; // UTF-16 BE
        else if (bytesRead >= 4 && buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
            encoding = Encoding.UTF32; // UTF-32 LE
        else
            encoding = Encoding.UTF8; // 默认UTF-8

        using var reader = new StreamReader(stream, encoding);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// 从文件流读取Markdown内容，并返回结构化信息（标题、内容、是否为有效MD）
    /// </summary>
    public static MarkdownContent ReadMarkdownFromStream(Stream stream)
    {
        var content = ReadTextFromStream(stream);
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

        // 简单判断是否为Markdown：检查是否包含至少一个标题或列表符号
        bool isMarkdown = lines.Any(line =>
            line.Trim().StartsWith("#") ||
            line.Trim().StartsWith("- ") ||
            line.Trim().StartsWith("* ") ||
            line.Trim().StartsWith("1. ") ||
            line.Contains("```") ||
            line.Contains("![") ||
            line.Contains("["));

        return new MarkdownContent
        {
            RawContent = content,
            IsMarkdown = isMarkdown,
            Title = lines.FirstOrDefault(line => line.Trim().StartsWith("# "))?.Trim().Substring(2).Trim() ?? "",
            LineCount = lines.Length,
            CharacterCount = content.Length
        };
    }

    /// <summary>
    /// 从text读取Markdown内容，并返回结构化信息（标题、内容、是否为有效MD）
    /// </summary>
    public static MarkdownContent ReadMarkdownFromText(string text)
    {
        var content = text;
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

        // 简单判断是否为Markdown：检查是否包含至少一个标题或列表符号
        bool isMarkdown = lines.Any(line =>
            line.Trim().StartsWith("#") ||
            line.Trim().StartsWith("- ") ||
            line.Trim().StartsWith("* ") ||
            line.Trim().StartsWith("1. ") ||
            line.Contains("```") ||
            line.Contains("![") ||
            line.Contains("["));

        return new MarkdownContent
        {
            RawContent = content,
            IsMarkdown = isMarkdown,
            Title = lines.FirstOrDefault(line => line.Trim().StartsWith("# "))?.Trim().Substring(2).Trim() ?? "",
            LineCount = lines.Length,
            CharacterCount = content.Length
        };
    }
}

/// <summary>
/// Markdown内容结构体，用于封装解析结果
/// </summary>
public class MarkdownContent
{
    public string RawContent { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsMarkdown { get; set; }
    public int LineCount { get; set; }
    public int CharacterCount { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
