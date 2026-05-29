using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Kevin.RAG.Tools;

public class RemoteFileDownloader : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed;

    public RemoteFileDownloader(int timeoutSeconds = 30)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
    }

    public RemoteFileDownloader(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Stream> DownloadAsStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));

        var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType;
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return stream;
    }

    public async Task<(Stream Stream, string FileName, string ContentType)> DownloadWithMetadataAsync(
        string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));

        var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = ExtractFileNameFromUrl(url) ?? ExtractFileNameFromContentDisposition(response.Content.Headers.ContentDisposition);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return (stream, fileName, contentType);
    }

    public async Task<byte[]> DownloadAsBytesAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));

        return await _httpClient.GetByteArrayAsync(uri);
    }

    public static string? DetermineFileTypeFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var uri = new Uri(url);
        var extension = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".html" or ".htm" => "text/html",
            ".txt" => "text/plain",
            ".md" => "text/markdown",
            _ => null
        };
    }

    public static DocumentType? DetermineDocumentType(string urlOrContentType)
    {
        var contentType = urlOrContentType;

        if (Uri.TryCreate(urlOrContentType, UriKind.Absolute, out _))
        {
            contentType = DetermineFileTypeFromUrl(urlOrContentType) ?? "";
        }

        return contentType.ToLowerInvariant() switch
        {
            var ct when ct.Contains("pdf") => DocumentType.Pdf,
            var ct when ct.Contains("word") || ct.Contains("doc") => DocumentType.Word,
            var ct when ct.Contains("excel") || ct.Contains("spreadsheet") || ct.Contains("xls") => DocumentType.Excel,
            var ct when ct.Contains("image") || ct.Contains("png") || ct.Contains("jpeg") || ct.Contains("jpg") || ct.Contains("gif") || ct.Contains("bmp") || ct.Contains("webp") => DocumentType.Image,
            var ct when ct.Contains("html") => DocumentType.Html,
            var ct when ct.Contains("text") || ct.Contains("markdown") || ct.Contains("md") => DocumentType.Text,
            _ => null
        };
    }

    private static string? ExtractFileNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            var fileName = Path.GetFileName(path);

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Contains('.'))
                return fileName;

            var query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, @"filename=([^&]+)", RegexOptions.IgnoreCase);
                if (match.Success)
                    return Uri.UnescapeDataString(match.Groups[1].Value);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string? ExtractFileNameFromContentDisposition(ContentDispositionHeaderValue? contentDisposition)
    {
        if (contentDisposition == null)
            return null;

        var fileName = contentDisposition.FileNameStar ?? contentDisposition.FileName;
        return fileName?.Trim('"');
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}

public enum DocumentType
{
    Pdf,
    Word,
    Excel,
    Image,
    Html,
    Text,
    Unknown
}

public static class RemoteDocumentLoader
{
    private static readonly Lazy<RemoteFileDownloader> _defaultDownloader = new(() => new RemoteFileDownloader());

    public static RemoteFileDownloader Default => _defaultDownloader.Value;

    public static async Task<string> LoadDocumentFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var (stream, fileName, contentType) = await Default.DownloadWithMetadataAsync(url, cancellationToken);

        using (stream)
        {
            var documentType = RemoteFileDownloader.DetermineDocumentType(contentType);

            return documentType switch
            {
                DocumentType.Pdf => await LoadPdfAsync(stream, cancellationToken),
                DocumentType.Word => await LoadWordAsync(stream, cancellationToken),
                DocumentType.Excel => await LoadExcelAsync(stream, fileName, cancellationToken),
                DocumentType.Image => await LoadImageAsync(stream, cancellationToken),
                DocumentType.Html => await LoadHtmlAsync(stream, cancellationToken),
                DocumentType.Text => await LoadTextAsync(stream, cancellationToken),
                _ => throw new NotSupportedException($"Document type '{contentType}' is not supported")
            };
        }
    }

    public static async Task<Stream> LoadDocumentStreamFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var (stream, _, contentType) = await Default.DownloadWithMetadataAsync(url, cancellationToken);
        return stream;
    }

    private static async Task<string> LoadPdfAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await Task.Run(() => PDFReader.ReadPdfToMarkdown(stream), cancellationToken);
    }

    private static async Task<string> LoadWordAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await Task.Run(() => WordReader.ReadParagraphs(stream), cancellationToken);
    }

    private static async Task<string> LoadExcelAsync(Stream stream, string fileName, CancellationToken cancellationToken)
    {
        return await Task.Run(() => ExcelReader.ReadExcelToMarkdown(stream, fileName), cancellationToken);
    }

    private static async Task<string> LoadImageAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return ImageReader.DescribeImage(memoryStream.ToArray());
    }

    private static async Task<string> LoadHtmlAsync(Stream stream, CancellationToken cancellationToken)
    {
        var content = await new StreamReader(stream).ReadToEndAsync(cancellationToken);
        return await HtmlReader.ExtractTextFromTextAsync(content);
    }

    private static async Task<string> LoadTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await new StreamReader(stream).ReadToEndAsync(cancellationToken);
    }
}