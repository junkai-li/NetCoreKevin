using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Actions;
using UglyToad.PdfPig.Annotations;
using UglyToad.PdfPig.Content;

namespace Kevin.RAG;
/// <summary>
/// PDF文件读取类，提供将PDF内容转换为Markdown的功能
/// </summary>
public class PDFReader
{
    /// <summary>
    /// 从文件路径读取PDF并转换为Markdown格式
    /// </summary>
    /// <param name="filePath">PDF文件路径</param>
    /// <returns>转换后的Markdown字符串</returns>
    public static string ReadPdfToMarkdown(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"PDF文件不存在: {filePath}");
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return ReadPdfToMarkdown(stream);
    }

    /// <summary>
    /// 从流读取PDF并转换为Markdown格式
    /// </summary>
    /// <param name="stream">PDF文件流</param>
    /// <returns>转换后的Markdown字符串</returns>
    public static string ReadPdfToMarkdown(Stream stream)
    {
        var markdown = new StringBuilder();
        string imageDirectory = Path.Combine(Path.GetTempPath(), "pdf_images_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(imageDirectory);

        try
        {
            using (var document = PdfDocument.Open(stream))
            {
                markdown.AppendLine($"# {GetDocumentTitle(document)}");
                markdown.AppendLine();

                int pageNumber = 1;
                foreach (var page in document.GetPages())
                {
                    markdown.AppendLine($"## 第 {pageNumber} 页");
                    markdown.AppendLine();

                    // 提取文本并进行布局分析
                    var words = page.GetWords().ToList();

                    // 使用DocStrLib分析处理段落和文本块
                    var blocks = ProcessPageText(page);

                    // 处理文本块，识别和应用Markdown格式
                    ProcessTextBlocks(blocks, markdown);

                    // 提取图片
                    var images = ExtractImagesFromPage(page, imageDirectory, pageNumber);
                    foreach (var imagePath in images)
                    {
                        markdown.AppendLine($"![图片]({imagePath})");
                        markdown.AppendLine();
                    }

                    // 处理表格
                    ProcessTables(page, markdown);

                    // 处理链接
                    ProcessLinks(page, markdown);

                    pageNumber++;
                }
            }

            // 后处理，清理并优化生成的Markdown
            string result = PostProcessMarkdown(markdown.ToString());
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"PDF转换Markdown失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 从页面中提取文本块
    /// </summary>
    private static List<TextBlock> ProcessPageText(Page page)
    {
        // ��用DocStrLib处理技术，提取和组织文本
        var words = page.GetWords().ToList();

        // 创建自定义文本块
        var blocks = new List<TextBlock>();

        // 基于文本的空间布局分析，将单词分组成块
        var wordGroups = GroupWordsIntoBlocks(words);

        foreach (var group in wordGroups)
        {
            string blockText = string.Join(" ", group.Select(w => w.Text));
            blocks.Add(new TextBlock(blockText, group));
        }

        return blocks;
    }

    /// <summary>
    /// 基于空间布局，将单词分组成文本块
    /// </summary>
    private static List<List<Word>> GroupWordsIntoBlocks(List<Word> words)
    {
        var result = new List<List<Word>>();
        if (!words.Any()) return result;

        // 使用简化的算法将相近的单词分组
        var currentGroup = new List<Word> { words[0] };
        double lastYBottom = words[0].BoundingBox.Bottom;

        for (int i = 1; i < words.Count; i++)
        {
            var word = words[i];

            // 如果Y坐标差异超过阈值，认为是新段落
            if (Math.Abs(word.BoundingBox.Bottom - lastYBottom) > 5)
            {
                result.Add(currentGroup);
                currentGroup = new List<Word>();
            }

            currentGroup.Add(word);
            lastYBottom = word.BoundingBox.Bottom;
        }

        if (currentGroup.Any())
            result.Add(currentGroup);

        return result;
    }

    /// <summary>
    /// 获取PDF文档标题
    /// </summary>
    private static string GetDocumentTitle(PdfDocument document)
    {
        if (document.Information.Title != null && !string.IsNullOrWhiteSpace(document.Information.Title.ToString()))
        {
            return document.Information.Title.ToString();
        }

        return "PDF文档";
    }

    /// <summary>
    /// 处理文本块，识别并应用Markdown格式
    /// </summary>
    /// <param name="textBlocks">文本块集合</param>
    /// <param name="markdown">Markdown构建器</param>
    private static void ProcessTextBlocks(List<TextBlock> textBlocks, StringBuilder markdown)
    {
        TextBlockAnalyzer analyzer = new TextBlockAnalyzer();
        List<FormattedTextBlock> formattedBlocks = analyzer.AnalyzeTextBlocks(textBlocks);

        foreach (var block in formattedBlocks)
        {
            string formattedText = block.Text;

            // 处理标题
            if (block.IsHeading)
            {
                int headingLevel = DetermineHeadingLevel(block);
                string headingMarker = new string('#', headingLevel);
                markdown.AppendLine($"{headingMarker} {formattedText}");
            }
            // 处理列表项
            else if (block.IsBulletListItem)
            {
                markdown.AppendLine($"* {formattedText}");
            }
            else if (block.IsNumberedListItem)
            {
                markdown.AppendLine($"1. {formattedText}");
            }
            // 处理引用块
            else if (block.IsQuoteBlock)
            {
                markdown.AppendLine($"> {formattedText}");
            }
            // 处理代码块
            else if (block.IsCodeBlock)
            {
                markdown.AppendLine("```");
                markdown.AppendLine(formattedText);
                markdown.AppendLine("```");
            }
            // 处理普通段落
            else
            {
                // 应用内联格式（粗体、斜体等）
                formattedText = ApplyInlineFormatting(formattedText, block);
                markdown.AppendLine(formattedText);
                markdown.AppendLine();
            }
        }
    }

    /// <summary>
    /// 确定标题级别
    /// </summary>
    private static int DetermineHeadingLevel(FormattedTextBlock block)
    {
        // 基于字体大小、字体粗细等确定标题级别
        var fontSize = block.FontSize;

        if (fontSize >= 20) return 1;
        if (fontSize >= 18) return 2;
        if (fontSize >= 16) return 3;
        if (fontSize >= 14) return 4;
        if (fontSize >= 12) return 5;
        return 6;
    }

    /// <summary>
    /// 应用内联格式（粗体、斜体等）
    /// </summary>
    private static string ApplyInlineFormatting(string text, FormattedTextBlock block)
    {
        // 处理粗体
        if (block.IsBold)
        {
            text = $"**{text}**";
        }

        // 处理斜体
        if (block.IsItalic)
        {
            text = $"*{text}*";
        }

        // 处理下划线
        if (block.IsUnderlined)
        {
            text = $"<u>{text}</u>";
        }

        return text;
    }

    /// <summary>
    /// 处理表格
    /// </summary>
    private static void ProcessTables(Page page, StringBuilder markdown)
    {
        var tableAnalyzer = new TableAnalyzer(page);
        var tables = tableAnalyzer.ExtractTables();

        foreach (var table in tables)
        {
            // 表头
            markdown.AppendLine("|" + string.Join("|", table.Headers.Select(h => h.Trim())) + "|");

            // 表头下的分隔符
            markdown.AppendLine("|" + string.Join("|", table.Headers.Select(_ => "---")) + "|");

            // 表格内容
            foreach (var row in table.Rows)
            {
                markdown.AppendLine("|" + string.Join("|", row.Select(cell => cell.Trim())) + "|");
            }

            markdown.AppendLine();
        }
    }

    /// <summary>
    /// 处理链接
    /// </summary>
    private static void ProcessLinks(Page page, StringBuilder markdown)
    {
        // 提取并处理页面中的链接
        var linkAnalyzer = new LinkAnalyzer(page);
        var links = linkAnalyzer.ExtractLinks();

        if (links.Any())
        {
            markdown.AppendLine("### 链接");
            markdown.AppendLine();

            int linkIndex = 1;
            foreach (var link in links)
            {
                markdown.AppendLine($"[{link.Text}]({link.Url})");
                linkIndex++;
            }

            markdown.AppendLine();
        }
    }

    /// <summary>
    /// 后处理Markdown，清理并优化格式
    /// </summary>
    private static string PostProcessMarkdown(string markdownText)
    {
        // 移除多余的空行
        markdownText = Regex.Replace(markdownText, @"\n{3,}", "\n\n");

        // 修正可能的Markdown格式问题
        markdownText = FixMarkdownFormatting(markdownText);

        return markdownText;
    }

    /// <summary>
    /// 修正Markdown格式问题
    /// </summary>
    private static string FixMarkdownFormatting(string markdownText)
    {
        // 确保标题前有空行
        markdownText = Regex.Replace(markdownText, @"([^\n])\n(#{1,6} )", "$1\n\n$2");

        // 确保列表项正确格式化
        markdownText = Regex.Replace(markdownText, @"([^\n])\n(\* |\d+\. )", "$1\n\n$2");

        return markdownText;
    }

    /// <summary>
    /// 从PDF页面提取图片
    /// </summary>
    /// <param name="page">PDF页面</param>
    /// <param name="outputDirectory">图片保存目录</param>
    /// <param name="pageNumber">页码</param>
    /// <returns>图片文件路径列表</returns>
    private static List<string> ExtractImagesFromPage(Page page, string outputDirectory, int pageNumber)
    {
        List<string> imagePaths = new List<string>();
        int imageIndex = 1;

        // 获取页面中的图片资源
        var images = page.GetImages();
        foreach (var image in images)
        {
            try
            {
                // 获取图像数据
                var imageData = image.RawBytes.ToArray();

                if (imageData is { Length: > 0 })
                {
                    // 保存图片
                    string imageFileName = $"page_{pageNumber}_image_{imageIndex}.png";
                    string imagePath = Path.Combine(outputDirectory, imageFileName);

                    File.WriteAllBytes(imagePath, imageData);
                    imagePaths.Add(imagePath);
                    imageIndex++;
                }
            }
            catch (Exception)
            {
                // 记录错误但继续处理
                continue;
            }
        }

        return imagePaths;
    }

    /// <summary>
    /// 从PDF文件提取所有图片
    /// </summary>
    /// <param name="pdfPath">PDF文件路径</param>
    /// <param name="outputDirectory">图片输出目录</param>
    /// <returns>���存的图片路径列表</returns>
    public static List<string> ExtractAllImages(string pdfPath, string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        List<string> savedImages = new List<string>();

        using (var document = PdfDocument.Open(pdfPath))
        {
            int pageNumber = 1;
            foreach (var page in document.GetPages())
            {
                var pageImages = ExtractImagesFromPage(page, outputDirectory, pageNumber);
                savedImages.AddRange(pageImages);
                pageNumber++;
            }
        }

        return savedImages;
    }
}

/// <summary>
/// 格式化文本块，包含文本及其格式信息
/// </summary>
public class FormattedTextBlock
{
    public string Text { get; set; } = string.Empty; // 初始化为空字符串，避免空引用
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderlined { get; set; }
    public bool IsHeading { get; set; }
    public bool IsBulletListItem { get; set; }
    public bool IsNumberedListItem { get; set; }
    public bool IsQuoteBlock { get; set; }
    public bool IsCodeBlock { get; set; }
    public double FontSize { get; set; }
}

/// <summary>
/// 自定义文本块类，存储文本内容和相关单词
/// </summary>
public class TextBlock
{
    public string Text { get; }
    public List<Word> Words { get; }

    public TextBlock(string text, List<Word> words)
    {
        Text = text;
        Words = words;
    }
}

/// <summary>
/// 文本块分析器，负责分析和识别文本格式
/// </summary>
public class TextBlockAnalyzer
{
    public List<FormattedTextBlock> AnalyzeTextBlocks(List<TextBlock> textBlocks)
    {
        List<FormattedTextBlock> formattedBlocks = new List<FormattedTextBlock>();

        foreach (var block in textBlocks)
        {
            // 使用TextBlock中存储的Words集合
            var words = block.Words;

            var formattedBlock = new FormattedTextBlock
            {
                Text = block.Text,
                FontSize = AnalyzeFontSize(words),
                IsBold = DetectBold(words),
                IsItalic = DetectItalic(words),
                IsUnderlined = DetectUnderline(words),
                IsHeading = DetectHeading(block),
                IsBulletListItem = DetectBulletListItem(block),
                IsNumberedListItem = DetectNumberedListItem(block),
                IsQuoteBlock = DetectQuoteBlock(block),
                IsCodeBlock = DetectCodeBlock(words)
            };

            formattedBlocks.Add(formattedBlock);
        }

        return formattedBlocks;
    }

    private double AnalyzeFontSize(List<Word> words)
    {
        if (!words.Any())
            return 12; // 默认字体大小

        // 通过单词中的字母获取字体大小
        return words.SelectMany(w => w.Letters).Average(l => l.FontSize);
    }

    private bool DetectBold(List<Word> words)
    {
        if (!words.Any())
            return false;

        // 根据字体名称判断是否粗体
        return words.SelectMany(w => w.Letters)
            .Any(l => l.FontName?.ToLower().Contains("bold") == true);
    }

    private bool DetectItalic(List<Word> words)
    {
        if (!words.Any())
            return false;

        // 根据字体名称判断是否斜体
        return words.SelectMany(w => w.Letters)
            .Any(l => l.FontName?.ToLower().Contains("italic") == true
                  || l.FontName?.ToLower().Contains("oblique") == true);
    }

    private bool DetectUnderline(List<Word> words)
    {
        // 检测下划线（更复杂，可能需要分析字形或特殊标记）
        return false; // 简化实现
    }

    private bool DetectHeading(TextBlock block)
    {
        if (!block.Words.Any())
            return false;

        // 通过字体��小和样式判断是否为标题
        var averageFontSize = block.Words.SelectMany(w => w.Letters)
            .Average(l => l.FontSize);

        return averageFontSize > 14 || (DetectBold(block.Words) && block.Text.Length < 100);
    }

    private bool DetectBulletListItem(TextBlock block)
    {
        // 检测无序列表项
        return block.Text.TrimStart().StartsWith("•") ||
               block.Text.TrimStart().StartsWith("-") ||
               block.Text.TrimStart().StartsWith("*");
    }

    private bool DetectNumberedListItem(TextBlock block)
    {
        // 检测有序列表项
        return Regex.IsMatch(block.Text.TrimStart(), @"^\d+\.\s");
    }

    private bool DetectQuoteBlock(TextBlock block)
    {
        // 检测引用块
        return block.Text.TrimStart().StartsWith(">");
    }

    private bool DetectCodeBlock(List<Word> words)
    {
        // 检测代码块（通过等宽字体或特定格式判断）
        return words.Any() &&
            words.SelectMany(w => w.Letters)
                .Where(l => l.FontName != null)
                .Any(l => (l.FontName??"").ToLower().Contains("mono") ||
                          (l.FontName ?? "").ToLower().Contains("courier"));
    }
}

/// <summary>
/// 表格分析器，负责识别和提取PDF中的表格
/// </summary>
public class TableAnalyzer
{
    private readonly Page _page;

    public TableAnalyzer(Page page)
    {
        _page = page;
    }

    public List<PdfTable> ExtractTables()
    {
        // 这里简化了表格检测和提取的实现
        // 实际上，表格检测是PDF解析中最复杂的部分之一
        // 需要分析线条、文本对齐和间距等多种因素

        var tables = new List<PdfTable>();

        // 检测表格的逻辑（示例）
        // 1. 查找水平和垂直线条
        // 2. 根据线条交叉点确定单元格
        // 3. 提取每个单元格中的文本

        return tables;
    }
}

/// <summary>
/// PDF表格表示
/// </summary>
public class PdfTable
{
    public List<string> Headers { get; set; } = new List<string>();
    public List<List<string>> Rows { get; set; } = new List<List<string>>();
}

/// <summary>
/// 链接分析器，负责提取PDF中的链接
/// </summary>
public class LinkAnalyzer
{
    private readonly Page _page;

    public LinkAnalyzer(Page page)
    {
        _page = page;
    }

    public List<PdfLink> ExtractLinks()
    {
        var links = new List<PdfLink>();

        // 提取页面中的链接
        foreach (var annotation in _page.ExperimentalAccess.GetAnnotations())
        {
            if (annotation.Type == AnnotationType.Link)
            {
                var link = new PdfLink
                {
                    Text = GetLinkText(annotation),
                    Url = GetLinkUrl(annotation)
                };

                if (!string.IsNullOrEmpty(link.Url))
                {
                    links.Add(link);
                }
            }
        }

        return links;
    }

    private string GetLinkText(Annotation annotation)
    {
        try
        {
            // 尝试获取链接文本
            return "链接";
        }
        catch
        {
            return "链接";
        }
    }

    private string GetLinkUrl(Annotation annotation)
    {
        try
        {
            // PdfPig中处理链接的方式
            if (annotation.Action is UriAction uriAction)
            {
                return uriAction.Uri?.ToString() ?? "#";
            }

            return "#";
        }
        catch
        {
            return "#";
        }
    }
}

/// <summary>
/// PDF链接表示
/// </summary>
public class PdfLink
{
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
