using ReverseMarkdown;

namespace ElBruno.MarkItDotNet.Converters;

/// <summary>
/// Converts HTML (.html, .htm) files to Markdown using the ReverseMarkdown library.
/// Handles headings, paragraphs, lists, links, images, bold/italic, code blocks, and tables.
/// </summary>
public class HtmlConverter : IMarkdownConverter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".html",
        ".htm"
    };

    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        SupportedExtensions.Contains(fileExtension);

    /// <inheritdoc />
    public async Task<string> ConvertAsync(Stream fileStream, string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var html = await reader.ReadToEndAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var converter = new Converter(new Config
        {
            UnknownTags = Config.UnknownTagsOption.PassThrough,
            RemoveComments = true,
            GithubFlavored = true,
            SmartHrefHandling = true
        });

        var markdown = converter.Convert(html);
        return markdown?.Trim() ?? string.Empty;
    }
}
