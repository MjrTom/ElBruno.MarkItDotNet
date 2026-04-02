using System.Runtime.CompilerServices;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ElBruno.MarkItDotNet.Converters;

/// <summary>
/// Converts PDF (.pdf) files to Markdown using UglyToad.PdfPig.
/// Extracts text content page by page, separated by horizontal rules.
/// Implements <see cref="IStreamingMarkdownConverter"/> for page-by-page streaming.
/// </summary>
public class PdfConverter : IStreamingMarkdownConverter
{
    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var document = PdfDocument.Open(fileStream);
        var sb = new StringBuilder();
        var pageCount = document.NumberOfPages;

        for (var i = 1; i <= pageCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = document.GetPage(i);
            var text = page.Text;

            if (i > 1)
            {
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                sb.AppendLine(text.Trim());
            }
        }

        return Task.FromResult(sb.ToString().TrimEnd());
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> ConvertStreamingAsync(
        Stream fileStream,
        string fileExtension,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var document = PdfDocument.Open(fileStream);
        var pageCount = document.NumberOfPages;

        for (var i = 1; i <= pageCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = document.GetPage(i);
            var text = page.Text;

            var sb = new StringBuilder();

            if (i > 1)
            {
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                sb.AppendLine(text.Trim());
            }

            yield return sb.ToString();
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
