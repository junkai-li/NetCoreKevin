using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using NetCore.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kevin.Common.Helper.FileHandleTools;

/// <summary>
/// PowerPoint(pptx) 解析器：把每页幻灯片的文本与备注按页拼成 Markdown，并提取页内图片字节。
/// <para>
/// 只支持 OOXML 的 .pptx（本质是 zip+XML）。.ppt（97-2003 二进制）无法用 OpenXml 打开，
/// 这里显式返回提示文案而不是抛异常，否则用户只会看到"读取失败"而不知道该换格式。
/// </para>
/// <para>
/// 已知取不到的内容：图表(SmartArt/chart)的数值、动画、以及纯图片页的文字——
/// 图表文本不在 DrawingML 的 a:t 节点里，纯图片页需要视觉模型看图，因此调用方应同时消费 <see cref="ExtractImages"/>。
/// </para>
/// </summary>
public class PptxReader
{
    private const string DrawingNamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";
    private const string RelationshipNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    /// <summary>单份文档最多提取的图片数：PPT 页数多、配图大，避免一次性把几十上百张图读进内存</summary>
    private const int MaxExtractedImages = 50;

    /// <summary>
    /// 读取幻灯片正文与备注，输出按页分节的 Markdown。
    /// 幻灯片顺序按 presentation.xml 的 SlideIdList 关系顺序（而非 zip 内文件顺序），保证与放映顺序一致。
    /// </summary>
    /// <param name="documentContents">pptx 文件流，需可定位</param>
    /// <returns>按页分节的 Markdown 文本；非 pptx 格式时返回提示文案</returns>
    public static string ReadSlidesToMarkdown(Stream documentContents)
    {
        if (documentContents == null || !documentContents.CanRead)
            throw new ArgumentException("无效的文件流");
        if (!documentContents.CanSeek)
            throw new InvalidOperationException("流不支持定位，无法用于 OpenXML");

        documentContents.Position = 0;

        // .ppt 是 OLE 复合文档，OpenXml 打不开；先按 zip 魔数(PK)拦下来给出可操作提示
        if (!LooksLikeZipPackage(documentContents))
        {
            return "（该文件不是 .pptx 格式：PowerPoint 97-2003 的 .ppt 无法直接解析，请在 PowerPoint 中另存为 .pptx 后重新上传）";
        }

        using PresentationDocument pptDoc = PresentationDocument.Open(documentContents, false);
        var slideIdList = pptDoc.PresentationPart?.Presentation?.SlideIdList;
        if (pptDoc.PresentationPart == null || slideIdList == null)
        {
            return string.Empty;
        }

        var markdownBuilder = new StringBuilder();
        var pageIndex = 0;

        foreach (SlideId slideId in slideIdList.Elements<SlideId>())
        {
            pageIndex++;
            markdownBuilder.AppendLine($"## 第 {pageIndex} 页");

            var relationshipId = slideId.RelationshipId?.Value;
            if (string.IsNullOrEmpty(relationshipId)
                || pptDoc.PresentationPart.GetPartById(relationshipId) is not SlidePart slidePart)
            {
                markdownBuilder.AppendLine();
                continue;
            }

            var paragraphs = ReadParagraphs(slidePart);
            if (paragraphs.Count == 0)
            {
                // 纯图片页没有可提取文本，明确告诉模型"这页只有图"，避免它以为文件是空的
                markdownBuilder.AppendLine("(本页无文字，内容为图片)");
            }
            foreach (var paragraph in paragraphs)
            {
                markdownBuilder.AppendLine(paragraph);
            }

            var notes = ReadNotes(slidePart);
            if (notes.Count > 0)
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("备注：");
                foreach (var note in notes)
                {
                    markdownBuilder.AppendLine(note);
                }
            }

            markdownBuilder.AppendLine();
        }

        return markdownBuilder.ToString().Trim();
    }

    /// <summary>
    /// 按页序提取幻灯片内嵌图片字节与 MIME，供多模态模型直接消费。
    /// 只处理 a:blip 的 r:embed 内嵌关系（外链图片不在提取范围），同一图片被多页复用时只取一次；
    /// 单张图片失败不影响后续提取，整体打开失败按"无图片"处理。
    /// </summary>
    /// <param name="documentContents">pptx 文件流，需可定位</param>
    /// <returns>图片字节与 MIME 列表</returns>
    public static List<(byte[] Bytes, string MimeType)> ExtractImages(Stream documentContents)
    {
        var images = new List<(byte[], string)>();
        if (documentContents == null || !documentContents.CanRead || !documentContents.CanSeek)
            return images;

        documentContents.Position = 0;

        try
        {
            using PresentationDocument pptDoc = PresentationDocument.Open(documentContents, false);
            var presentationPart = pptDoc.PresentationPart;
            var slideIdList = presentationPart?.Presentation?.SlideIdList;
            if (presentationPart == null || slideIdList == null)
                return images;

            // 母版/版式里的图片会通过关系被多页引用，按 part 去重避免同一张图重复占配额
            var visitedParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (SlideId slideId in slideIdList.Elements<SlideId>())
            {
                if (images.Count >= MaxExtractedImages) break;

                var relationshipId = slideId.RelationshipId?.Value;
                if (string.IsNullOrEmpty(relationshipId)
                    || presentationPart.GetPartById(relationshipId) is not SlidePart slidePart)
                {
                    continue;
                }

                XmlDocument xmlDoc = new XmlDocument();
                using (var slideStream = slidePart.GetStream())
                {
                    xmlDoc.Load(slideStream);
                }

                var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsManager.AddNamespace("a", DrawingNamespace);
                nsManager.AddNamespace("r", RelationshipNamespace);

                var blips = xmlDoc.SelectNodes("//a:blip", nsManager);
                if (blips == null) continue;

                foreach (XmlNode blip in blips)
                {
                    if (images.Count >= MaxExtractedImages) break;

                    string imageId = blip.Attributes?["r:embed"]?.Value;
                    if (string.IsNullOrEmpty(imageId)) continue;

                    try
                    {
                        if (slidePart.GetPartById(imageId) is not ImagePart imagePart) continue;
                        if (!visitedParts.Add(imagePart.Uri.OriginalString)) continue;

                        using var imageStream = imagePart.GetStream();
                        using var ms = new MemoryStream();
                        imageStream.CopyTo(ms);
                        var bytes = ms.ToArray();
                        if (bytes.Length == 0) continue;

                        var mime = imagePart.ContentType;
                        if (string.IsNullOrWhiteSpace(mime))
                            mime = FileHelper.SniffImageMime(bytes);
                        if (!string.IsNullOrWhiteSpace(mime))
                            images.Add((bytes, mime));
                    }
                    catch (Exception)
                    {
                        // 关系ID失效等单张图片异常跳过，不影响整份文档
                        continue;
                    }
                }
            }
        }
        catch (Exception)
        {
            // 文档打开失败等由调用方按"无图片"处理
        }

        return images;
    }

    /// <summary>
    /// 取单页正文：按 a:p 段落聚合 a:t 文本节点，保留段落换行（标题与正文因此不会被粘成一行）。
    /// </summary>
    private static List<string> ReadParagraphs(SlidePart slidePart)
    {
        var paragraphs = new List<string>();

        XmlDocument xmlDoc = new XmlDocument();
        using (var slideStream = slidePart.GetStream())
        {
            xmlDoc.Load(slideStream);
        }

        var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("a", DrawingNamespace);

        var nodes = xmlDoc.SelectNodes("//a:p", nsManager);
        if (nodes == null) return paragraphs;

        foreach (XmlNode paragraph in nodes)
        {
            var text = ExtractNodeText(paragraph, nsManager);
            if (text.Length > 0) paragraphs.Add(text);
        }

        return paragraphs;
    }

    /// <summary>
    /// 取单页备注（讲稿）：备注页结构与正文相同，文本同样在 a:t 里。
    /// 备注页自带的页码占位符会留下纯数字段落，过滤掉减少噪音。
    /// </summary>
    private static List<string> ReadNotes(SlidePart slidePart)
    {
        var notes = new List<string>();

        try
        {
            var notesPart = slidePart.NotesSlidePart;
            if (notesPart == null) return notes;

            XmlDocument xmlDoc = new XmlDocument();
            using (var notesStream = notesPart.GetStream())
            {
                xmlDoc.Load(notesStream);
            }

            var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("a", DrawingNamespace);

            var nodes = xmlDoc.SelectNodes("//a:p", nsManager);
            if (nodes == null) return notes;

            foreach (XmlNode paragraph in nodes)
            {
                var text = ExtractNodeText(paragraph, nsManager);
                if (text.Length == 0 || text.All(char.IsDigit)) continue;
                notes.Add(text);
            }
        }
        catch (Exception)
        {
            // 备注缺失或损坏不影响幻灯片正文解析
        }

        return notes;
    }

    /// <summary>拼接一个段落内全部 a:t 节点的文本（一个段落可能被拆成多个 run）</summary>
    private static string ExtractNodeText(XmlNode paragraph, XmlNamespaceManager nsManager)
    {
        var textNodes = paragraph.SelectNodes(".//a:t", nsManager);
        if (textNodes == null) return string.Empty;

        var builder = new StringBuilder();
        foreach (XmlNode textNode in textNodes)
        {
            builder.Append(textNode.InnerText);
        }
        return builder.ToString().Trim();
    }

    /// <summary>按 zip 魔数判断是否 OOXML 包：pptx/docx/xlsx 均以 PK 开头</summary>
    private static bool LooksLikeZipPackage(Stream stream)
    {
        var header = new byte[2];
        stream.Position = 0;
        var read = stream.Read(header, 0, 2);
        stream.Position = 0;
        return read == 2 && header[0] == 0x50 && header[1] == 0x4B;
    }
}
