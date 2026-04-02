namespace ElBruno.MarkItDotNet;

/// <summary>
/// Extended converter contract that supports asynchronous, chunk-by-chunk streaming
/// (e.g., page-by-page for PDFs). Converters that implement this interface are also
/// backward-compatible with <see cref="IMarkdownConverter.ConvertAsync"/>.
/// </summary>
public interface IStreamingMarkdownConverter : IMarkdownConverter
{
    /// <summary>
    /// Converts the content of a stream to Markdown, yielding chunks asynchronously.
    /// </summary>
    /// <param name="fileStream">The input stream containing file content.</param>
    /// <param name="fileExtension">File extension including the leading dot (e.g., ".pdf").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of Markdown chunks.</returns>
    IAsyncEnumerable<string> ConvertStreamingAsync(
        Stream fileStream,
        string fileExtension,
        CancellationToken cancellationToken = default);
}
