using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Kevin.RAG.Tools;

public class ExcelReader
{
    public static string ReadExcelToMarkdown(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Excel file not found: {filePath}");

        using var stream = File.OpenRead(filePath);
        return ReadExcelToMarkdown(stream, Path.GetFileName(filePath));
    }

    public static string ReadExcelToMarkdown(Stream stream, string fileName = "Sheet")
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        stream.Position = 0;
        var workbook = WorkbookFactory.Create(stream);

        return ConvertWorkbookToMarkdown(workbook, fileName);
    }

    public static async Task<string> ReadExcelToMarkdownAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => ReadExcelToMarkdown(filePath), cancellationToken);
    }

    public static async Task<string> ReadExcelToMarkdownAsync(Stream stream, string fileName = "Sheet", CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => ReadExcelToMarkdown(stream, fileName), cancellationToken);
    }

    public static async Task<string> ReadExcelToMarkdownFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var (stream, fileName, _) = await RemoteDocumentLoader.Default.DownloadWithMetadataAsync(url, cancellationToken);
        using (stream)
        {
            return await ReadExcelToMarkdownAsync(stream, fileName, cancellationToken);
        }
    }

    private static string ConvertWorkbookToMarkdown(IWorkbook workbook, string fileName)
    {
        var markdown = new StringBuilder();
        markdown.AppendLine($"# {Path.GetFileNameWithoutExtension(fileName)}");
        markdown.AppendLine();

        int sheetCount = workbook.NumberOfSheets;

        if (sheetCount == 0)
        {
            return markdown.ToString();
        }

        if (sheetCount == 1)
        {
            var sheet = workbook.GetSheetAt(0);
            markdown.AppendLine($"## {sheet.SheetName}");
            markdown.AppendLine();
            ConvertSheetToMarkdown(sheet, markdown);
        }
        else
        {
            for (int i = 0; i < sheetCount; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                markdown.AppendLine($"## Sheet {i + 1}: {sheet.SheetName}");
                markdown.AppendLine();
                ConvertSheetToMarkdown(sheet, markdown);
                markdown.AppendLine();
            }
        }

        return markdown.ToString();
    }

    private static void ConvertSheetToMarkdown(ISheet sheet, StringBuilder markdown)
    {
        if (sheet == null)
            return;

        var firstRowNum = sheet.FirstRowNum;
        var lastRowNum = sheet.LastRowNum;

        if (firstRowNum == lastRowNum && sheet.GetRow(firstRowNum) == null)
            return;

        var processedRows = new HashSet<int>();

        for (int rowNum = firstRowNum; rowNum <= lastRowNum; rowNum++)
        {
            var row = sheet.GetRow(rowNum);
            if (row == null)
                continue;

            if (processedRows.Contains(rowNum))
                continue;

            processedRows.Add(rowNum);

            var cells = GetRowCells(row);
            if (cells.Count == 0)
                continue;

            bool isHeaderRow = DetectHeaderRow(sheet, rowNum, cells);

            if (isHeaderRow && rowNum == firstRowNum)
            {
                markdown.AppendLine("| " + string.Join(" | ", cells.Select(c => EscapeMarkdown(c))) + " |");
                markdown.AppendLine("| " + string.Join("|", cells.Select(_ => "---")) + " |");
            }
            else
            {
                markdown.AppendLine("| " + string.Join(" | ", cells.Select(c => EscapeMarkdown(c))) + " |");
            }
        }
    }

    private static List<string> GetRowCells(IRow row)
    {
        var cells = new List<string>();
        if (row == null)
            return cells;

        var lastCellNum = row.LastCellNum;

        for (int cellNum = 0; cellNum < lastCellNum; cellNum++)
        {
            var cell = row.GetCell(cellNum);
            cells.Add(GetCellValue(cell));
        }

        while (cells.Count > 0 && string.IsNullOrWhiteSpace(cells[^1]))
        {
            cells.RemoveAt(cells.Count - 1);
        }

        return cells;
    }

    private static string GetCellValue(ICell? cell)
    {
        if (cell == null)
            return string.Empty;

        switch (cell.CellType)
        {
            case CellType.String:
                return cell.StringCellValue ?? string.Empty;

            case CellType.Numeric:
                if (DateUtil.IsCellDateFormatted(cell))
                {
                    var dateValue = DateTime.FromOADate(cell.NumericCellValue);
                    return dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                }
                var numericValue = cell.NumericCellValue;
                if (numericValue == Math.Floor(numericValue))
                {
                    return ((long)numericValue).ToString();
                }
                return numericValue.ToString("G10");

            case CellType.Boolean:
                return cell.BooleanCellValue ? "TRUE" : "FALSE";

            case CellType.Formula:
                try
                {
                    return cell.NumericCellValue.ToString();
                }
                catch
                {
                    return cell.StringCellValue ?? string.Empty;
                }

            case CellType.Error:
                return $"[Error: {cell.ErrorCellValue}]";

            case CellType.Blank:
            default:
                return string.Empty;
        }
    }

    private static bool DetectHeaderRow(ISheet sheet, int rowNum, List<string> cells)
    {
        if (cells.Count == 0)
            return false;

        var row = sheet.GetRow(rowNum);
        if (row == null)
            return false;

        int boldCount = 0;
        int totalCells = 0;

        foreach (var cell in row.Cells)
        {
            if (cell.CellType == CellType.String || cell.CellType == CellType.Formula)
            {
                totalCells++;
                var cellStyle = cell.CellStyle;
                if (cellStyle != null && cellStyle.FontIndex >= 0)
                {
                    try
                    {
                        var font = sheet.Workbook.GetFontAt(cellStyle.FontIndex);
                        if (font != null && font.IsBold)
                            boldCount++;
                    }
                    catch { }
                }
            }
        }

        if (totalCells > 0 && boldCount >= totalCells / 2)
            return true;

        bool allString = cells.All(c => !string.IsNullOrWhiteSpace(c) && !IsNumericString(c));

        if (allString && cells.Count > 0)
        {
            var firstCellLower = cells[0].ToLowerInvariant();
            if (firstCellLower is "name" or "title" or "id" or "key" or "value" or "date" or "description")
                return true;
        }

        return false;
    }

    private static bool IsNumericString(string str)
    {
        return double.TryParse(str.Trim(), out _);
    }

    private static string EscapeMarkdown(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = text.Replace("|", "\\|")
                   .Replace("\n", " ")
                   .Replace("\r", "");

        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }

    public static bool IsExcelFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension is ".xls" or ".xlsx" or ".xlsm" or ".xlsb";
    }

    public static bool IsExcelFileByContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return contentType.ToLowerInvariant().Contains("excel") ||
               contentType.ToLowerInvariant().Contains("spreadsheet") ||
               contentType.ToLowerInvariant().Contains("xls");
    }

    public static List<List<string>> ReadExcelAsTable(string filePath, int sheetIndex = 0)
    {
        using var stream = File.OpenRead(filePath);
        var workbook = WorkbookFactory.Create(stream);
        var sheet = workbook.GetSheetAt(sheetIndex);

        return ReadSheetAsTable(sheet);
    }

    public static List<List<string>> ReadExcelAsTable(Stream stream, int sheetIndex = 0)
    {
        stream.Position = 0;
        var workbook = WorkbookFactory.Create(stream);
        var sheet = workbook.GetSheetAt(sheetIndex);

        return ReadSheetAsTable(sheet);
    }

    private static List<List<string>> ReadSheetAsTable(ISheet sheet)
    {
        var table = new List<List<string>>();

        if (sheet == null)
            return table;

        for (int rowNum = sheet.FirstRowNum; rowNum <= sheet.LastRowNum; rowNum++)
        {
            var row = sheet.GetRow(rowNum);
            if (row == null)
                continue;

            var cells = GetRowCells(row);
            if (cells.Count > 0)
            {
                table.Add(cells);
            }
        }

        return table;
    }
}