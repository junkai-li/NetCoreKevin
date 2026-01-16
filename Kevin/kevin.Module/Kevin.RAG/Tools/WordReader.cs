using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;

namespace Kevin.RAG;

public class WordReader
{
    public static string ReadParagraphs(Stream documentContents)
    {
        // Open the document.
        if(documentContents == null || !documentContents.CanRead)
            throw new ArgumentException("文件流无效");

        // ✅ 关键步骤1：确保流可定位且支持 Seek
        if (!documentContents.CanSeek)
            throw new InvalidOperationException("流不支持定位，无法用于 OpenXML");

        // ✅ 关键步骤2：重置流至起始位置（修复90%的异常）
        long originalPosition = documentContents.Position;
        documentContents.Position = 0;
        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentContents, false);
        if (wordDoc.MainDocumentPart == null)
        {
            return string.Empty;
        }

        // Create an XmlDocument to hold the document contents and load the document contents into the XmlDocument.
        XmlDocument xmlDoc = new XmlDocument();
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        nsManager.AddNamespace("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        nsManager.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        nsManager.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
        nsManager.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

        xmlDoc.Load(wordDoc.MainDocumentPart.GetStream());

        // Create a StringBuilder to store markdown text
        StringBuilder markdownBuilder = new StringBuilder();

        // Process tables
        ProcessTables(xmlDoc, nsManager, markdownBuilder);

        // Process content including paragraphs, lists, hyperlinks, etc.
        ProcessContent(xmlDoc, nsManager, markdownBuilder, wordDoc);

        return markdownBuilder.ToString().Trim();
    }

    private static void ProcessContent(XmlDocument xmlDoc, XmlNamespaceManager nsManager,
        StringBuilder markdownBuilder, WordprocessingDocument wordDoc)
    {
        // Get all block-level elements (paragraphs, tables, etc.)
        XmlNodeList? blocks = xmlDoc.SelectNodes("//w:p | //w:tbl", nsManager);
        if (blocks == null)
        {
            return;
        }

        // Track list context for proper nesting
        Dictionary<int, int> listNumbering = new Dictionary<int, int>();
        int? currentListId = null;
        int currentLevel = 0;

        // Process each block element
        foreach (XmlNode block in blocks)
        {
            if (block.Name == "w:tbl")
            {
                // Tables are processed separately
                continue;
            }

            // It's a paragraph
            XmlNode paragraph = block;

            // Select all text nodes in the paragraph and continue if none found.
            XmlNodeList? runs = paragraph.SelectNodes(".//w:r", nsManager);
            if (runs == null || runs.Count == 0)
            {
                continue;
            }

            // Process the paragraph for special formatting
            string formattedParagraph = ProcessParagraph(paragraph, nsManager, wordDoc);

            // Skip empty paragraphs
            if (string.IsNullOrWhiteSpace(formattedParagraph))
            {
                continue;
            }

            // Check for paragraph style to determine if it's a heading
            XmlNode? styleNode = paragraph.SelectSingleNode(".//w:pStyle", nsManager);
            if (styleNode != null && styleNode.Attributes != null)
            {
                var styleVal = styleNode.Attributes["w:val"]?.Value;
                if (styleVal != null && styleVal.StartsWith("Heading"))
                {
                    // Try to extract heading level
                    if (int.TryParse(styleVal.Replace("Heading", ""), out int headingLevel))
                    {
                        string headingMarkers = new string('#', headingLevel);
                        markdownBuilder.AppendLine($"{headingMarkers} {formattedParagraph}");
                        markdownBuilder.AppendLine(); // Add an empty line after heading
                        continue;
                    }
                }

                // Check for quote style
                if (styleVal != null && styleVal.Contains("Quote"))
                {
                    markdownBuilder.AppendLine($"> {formattedParagraph}");
                    markdownBuilder.AppendLine();
                    continue;
                }
            }

            // Check for list items
            XmlNode? numPrNode = paragraph.SelectSingleNode(".//w:numPr", nsManager);
            if (numPrNode != null)
            {
                ProcessListItem(numPrNode, nsManager, markdownBuilder, formattedParagraph,
                    ref listNumbering, ref currentListId, ref currentLevel);
                continue;
            }
            else
            {
                // Reset list tracking when we're not in a list anymore
                currentListId = null;
                currentLevel = 0;
            }

            // Regular paragraph
            markdownBuilder.AppendLine(formattedParagraph);
            markdownBuilder.AppendLine(); // Add empty line between paragraphs
        }
    }

    private static string ProcessParagraph(XmlNode paragraph, XmlNamespaceManager nsManager,
        WordprocessingDocument wordDoc)
    {
        StringBuilder paragraphText = new StringBuilder();

        XmlNodeList? runs = paragraph.SelectNodes(".//w:r", nsManager);
        if (runs == null)
        {
            return string.Empty;
        }

        foreach (XmlNode run in runs)
        {
            // Process text
            XmlNode? textNode = run.SelectSingleNode(".//w:t", nsManager);
            string text = textNode?.InnerText ?? string.Empty;

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            // Check for text formatting
            bool isBold = run.SelectSingleNode(".//w:b", nsManager) != null;
            bool isItalic = run.SelectSingleNode(".//w:i", nsManager) != null;
            bool isUnderline = run.SelectSingleNode(".//w:u", nsManager) != null;
            bool isStrike = run.SelectSingleNode(".//w:strike", nsManager) != null;

            // Apply formatting
            if (isStrike)
            {
                text = $"~~{text}~~";
            }

            if (isBold && isItalic)
            {
                text = $"***{text}***";
            }
            else if (isBold)
            {
                text = $"**{text}**";
            }
            else if (isItalic)
            {
                text = $"*{text}*";
            }

            // Check for hyperlinks
            XmlNode? parent = run.ParentNode;
            if (parent != null && parent.Name == "w:hyperlink" && parent.Attributes != null)
            {
                string? id = parent.Attributes["r:id"]?.Value;
                if (!string.IsNullOrEmpty(id) && wordDoc.MainDocumentPart != null)
                {
                    var hyperlink = wordDoc.MainDocumentPart.HyperlinkRelationships.FirstOrDefault(h => h.Id == id);
                    if (hyperlink != null)
                    {
                        text = $"[{text}]({hyperlink.Uri})";
                    }
                }
            }

            // Check for images
            XmlNode? drawing = run.SelectSingleNode(".//w:drawing", nsManager);
            if (drawing != null)
            {
                XmlNode? blipNode = drawing.SelectSingleNode(".//a:blip", nsManager);
                if (blipNode?.Attributes != null)
                {
                    string? imageId = blipNode.Attributes["r:embed"]?.Value;
                    if (!string.IsNullOrEmpty(imageId) && wordDoc.MainDocumentPart != null)
                    {
                        var imagePart = wordDoc.MainDocumentPart.GetPartById(imageId);
                        if (imagePart != null)
                        {
                            string altText = "image";
                            XmlNode? docPr = drawing.SelectSingleNode(".//wp:docPr", nsManager);
                            if (docPr?.Attributes != null)
                            {
                                altText = docPr.Attributes["name"]?.Value ?? "image";
                            }

                            // For simplicity, we're not embedding the actual image data
                            text = $"![{altText}](image-reference-{imageId})";
                        }
                    }
                }
            }

            paragraphText.Append(text);
        }

        return paragraphText.ToString();
    }

    private static void ProcessListItem(XmlNode numPrNode, XmlNamespaceManager nsManager,
        StringBuilder markdownBuilder, string text,
        ref Dictionary<int, int> listNumbering,
        ref int? currentListId, ref int currentLevel)
    {
        XmlNode? numIdNode = numPrNode.SelectSingleNode(".//w:numId", nsManager);
        XmlNode? ilvlNode = numPrNode.SelectSingleNode(".//w:ilvl", nsManager);

        if (numIdNode?.Attributes != null && ilvlNode?.Attributes != null)
        {
            int numId = int.Parse(numIdNode.Attributes["w:val"]?.Value ?? "0");
            int level = int.Parse(ilvlNode.Attributes["w:val"]?.Value ?? "0");

            // If this is a new list or level changed, update counters
            if (currentListId != numId)
            {
                currentListId = numId;
                listNumbering.Clear();
            }

            // Track the numbering for this level
            if (!listNumbering.ContainsKey(level))
            {
                listNumbering[level] = 0;
            }

            currentLevel = level;

            // Indentation based on level
            string indent = new string(' ', level * 2);

            // Default to bullet list, but could be improved to detect ordered lists
            string marker = "- ";

            // For ordered lists, this would need to be modified to use the actual numbering
            // from the Word document's numbering definitions
            if (numId % 2 == 0) // Simple heuristic: even numId might be ordered lists
            {
                listNumbering[level]++;
                marker = $"{listNumbering[level]}. ";
            }

            markdownBuilder.AppendLine($"{indent}{marker}{text}");
        }
        else
        {
            // Fallback for simple list items
            markdownBuilder.AppendLine($"- {text}");
        }

        markdownBuilder.AppendLine();
    }

    private static void ProcessTables(XmlDocument xmlDoc, XmlNamespaceManager nsManager, StringBuilder markdownBuilder)
    {
        XmlNodeList? tables = xmlDoc.SelectNodes("//w:tbl", nsManager);
        if (tables == null)
        {
            return;
        }

        foreach (XmlNode table in tables)
        {
            // Get all rows in the table
            XmlNodeList? rows = table.SelectNodes(".//w:tr", nsManager);
            if (rows == null || rows.Count == 0)
            {
                continue;
            }

            List<List<string>> tableData = new List<List<string>>();

            // Process each row
            foreach (XmlNode row in rows)
            {
                List<string> rowData = new List<string>();

                // Process each cell in the row
                XmlNodeList? cells = row.SelectNodes(".//w:tc", nsManager);
                if (cells != null)
                {
                    foreach (XmlNode cell in cells)
                    {
                        // Get text content of the cell
                        StringBuilder cellText = new StringBuilder();
                        XmlNodeList? paragraphs = cell.SelectNodes(".//w:p", nsManager);
                        if (paragraphs != null)
                        {
                            foreach (XmlNode para in paragraphs)
                            {
                                XmlNodeList? texts = para.SelectNodes(".//w:t", nsManager);
                                if (texts != null)
                                {
                                    foreach (XmlNode text in texts)
                                    {
                                        cellText.Append(text.InnerText);
                                    }
                                }

                                cellText.Append(" "); // Space between paragraphs in a cell
                            }
                        }

                        rowData.Add(cellText.ToString().Trim());
                    }
                }

                if (rowData.Any())
                {
                    tableData.Add(rowData);
                }
            }

            // Now render the table in Markdown
            if (tableData.Any())
            {
                int columnCount = tableData.Max(row => row.Count);

                // Header row
                markdownBuilder.AppendLine("|" + string.Join("|",
                    tableData[0].Concat(Enumerable.Repeat("", Math.Max(0, columnCount - tableData[0].Count)))
                        .Select(cell => $" {cell} ")) + "|");

                // Separator row
                markdownBuilder.AppendLine("|" + string.Join("|", Enumerable.Repeat(" --- ", columnCount)) + "|");

                // Data rows
                for (int i = 1; i < tableData.Count; i++)
                {
                    List<string> rowData = tableData[i];
                    markdownBuilder.AppendLine("|" + string.Join("|",
                        rowData.Concat(Enumerable.Repeat("", Math.Max(0, columnCount - rowData.Count)))
                            .Select(cell => $" {cell} ")) + "|");
                }

                markdownBuilder.AppendLine();
            }
        }
    }
}