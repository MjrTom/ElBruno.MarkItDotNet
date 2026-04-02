using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ElBruno.MarkItDotNet.Converters;

/// <summary>
/// Converts PDF (.pdf) files to Markdown using UglyToad.PdfPig.
/// Extracts text content page by page, separated by horizontal rules.
/// </summary>
public class PdfConverter : IMarkdownConverter
{
    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Task<string> ConvertAsync(Stream fileStream, string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var document = PdfDocument.Open(fileStream);
        var sb = new StringBuilder();
        var pageCount = document.NumberOfPages;

        for (var i = 1; i <= pageCount; i++)
        {
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
}
