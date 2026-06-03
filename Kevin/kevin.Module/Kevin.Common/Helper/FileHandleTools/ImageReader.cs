using SixLabors.ImageSharp;
using System;
using System.Collections.Generic; 
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kevin.Common.Helper.FileHandleTools;

public class ImageReader
{
    public static string ExtractTextFromImage(string imagePath)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Image file not found: {imagePath}");

        var imageBytes = File.ReadAllBytes(imagePath);
        return ExtractTextFromImage(imageBytes);
    }

    public static string ExtractTextFromImage(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
            return string.Empty;

        try
        {
            return TesseractOcr(imageBytes);
        }
        catch (Exception ex)
        {
            return $"OCR识别失败: {ex.Message}";
        }
    }

    public static string ExtractTextFromImage(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return ExtractTextFromImage(memoryStream.ToArray());
    }

    public static async Task<string> ExtractTextFromImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => ExtractTextFromImage(imagePath), cancellationToken);
    }

    public static async Task<string> ExtractTextFromImageAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => ExtractTextFromImage(imageBytes), cancellationToken);
    }

    public static async Task<string> ExtractTextFromImageAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return await ExtractTextFromImageAsync(memoryStream.ToArray(), cancellationToken);
    }

    public static string DescribeImage(byte[] imageBytes, string language = "eng")
    {
        if (imageBytes == null || imageBytes.Length == 0)
            return string.Empty;

        try
        {
            var text = ExtractTextFromImage(imageBytes);
            var imageInfo = GetImageInfo(imageBytes);

            var description = new StringBuilder();
            description.AppendLine("## 图片内容");
            description.AppendLine();

            if (!string.IsNullOrWhiteSpace(text) && !text.Contains("暂不可用"))
            {
                description.AppendLine("### 识别文本 (OCR)");
                description.AppendLine();
                description.AppendLine(text);
                description.AppendLine();
            }
            else if (text.Contains("暂不可用"))
            {
                description.AppendLine("### 图片文字识别 (OCR)");
                description.AppendLine();
                description.AppendLine(text);
                description.AppendLine();
            }

            description.AppendLine("### 图片信息");
            description.AppendLine();
            foreach (var info in imageInfo)
            {
                description.AppendLine($"- **{info.Key}**: {info.Value}");
            }

            return description.ToString();
        }
        catch (Exception ex)
        {
            return $"![图片](无法识别图片内容: {ex.Message})";
        }
    }

    public static string DescribeImage(Stream stream, string language = "eng")
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return DescribeImage(memoryStream.ToArray(), language);
    }

    public static async Task<string> DescribeImageAsync(byte[] imageBytes, string language = "eng", CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => DescribeImage(imageBytes, language), cancellationToken);
    }

    public static async Task<string> DescribeImageAsync(Stream stream, string language = "eng", CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return await DescribeImageAsync(memoryStream.ToArray(), language, cancellationToken);
    }

    private static string TesseractOcr(byte[] imageBytes, string language = "eng")
    {
        var tessdataPath = GetTessdataPath();
        if (!Directory.Exists(tessdataPath) || !File.Exists(Path.Combine(tessdataPath, $"{language}.traineddata")))
        {
            return "OCR功能暂不可用：缺少语言训练数据文件。请确保tessdata文件夹中包含相应的.traineddata文件。";
        }

        try
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"tess_{Guid.NewGuid()}.png");
            File.WriteAllBytes(tempFile, imageBytes);

            try
            {
                using var engine = new Tesseract.TesseractEngine(tessdataPath, language, Tesseract.EngineMode.Default);
                using var img = Tesseract.Pix.LoadFromFile(tempFile);
                using var page = engine.Process(img);

                return page.GetText();
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        catch (Exception ex)
        {
            return $"OCR识别失败: {ex.Message}";
        }
    }

    private static string GetTessdataPath()
    {
        var paths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "tessdata"),
            "C:\\Program Files\\Tesseract-OCR\\tessdata",
            "/usr/share/tesseract-ocr/tessdata",
            "/usr/local/share/tessdata"
        };

        foreach (var path in paths)
        {
            if (Directory.Exists(path))
                return path;
        }

        return paths[0];
    }

    private static Dictionary<string, string> GetImageInfo(byte[] imageBytes)
    {
        var info = new Dictionary<string, string>();

        try
        {
            using var image = Image.Load(imageBytes);
            info["宽度"] = $"{image.Width} px";
            info["高度"] = $"{image.Height} px";
            info["格式"] = image.Metadata.DecodedImageFormat?.Name ?? "Unknown";

            if (image.Metadata.ExifProfile != null)
            {
                info["状态"] = "存在EXIF信息";
            }
        }
        catch
        {
            info["状态"] = "无法读取图片信息";
        }

        return info;
    }

    public static bool IsImageFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".webp" or ".tiff" or ".tif";
    }

    public static bool IsImageFileByContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return contentType.ToLowerInvariant().Contains("image");
    }

    public static string ImageToBase64(byte[] imageBytes)
    {
        return Convert.ToBase64String(imageBytes);
    }

    public static byte[] Base64ToImage(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return Array.Empty<byte>();

        return Convert.FromBase64String(base64String);
    }

    public static string GetImageMarkdown(byte[] imageBytes, string altText = "图片")
    {
        var base64 = ImageToBase64(imageBytes);

        string mimeType = "image/png";
        try
        {
            using var image = Image.Load(imageBytes);
            mimeType = image.Metadata.DecodedImageFormat?.Name?.ToLowerInvariant() switch
            {
                "jpeg" or "jpg" => "image/jpeg",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "webp" => "image/webp",
                _ => "image/png"
            };
        }
        catch { }

        return $"![{altText}](data:{mimeType};base64,{base64})";
    }

    public static string GetImageMarkdown(string filePath, string altText = "图片")
    {
        var imageBytes = File.ReadAllBytes(filePath);
        return GetImageMarkdown(imageBytes, altText);
    }
}