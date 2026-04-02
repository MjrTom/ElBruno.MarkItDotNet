namespace ElBruno.MarkItDotNet;

/// <summary>
/// Converts various file formats to Markdown.
/// </summary>
public class MarkdownConverter
{
    /// <summary>
    /// Converts the content of a file to Markdown.
    /// </summary>
    /// <param name="filePath">Path to the file to convert.</param>
    /// <returns>The Markdown representation of the file content.</returns>
    public string ConvertToMarkdown(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".txt" => ConvertTextFile(filePath),
            _ => throw new NotSupportedException($"File format '{extension}' is not yet supported.")
        };
    }

    private static string ConvertTextFile(string filePath) => File.ReadAllText(filePath);
}
