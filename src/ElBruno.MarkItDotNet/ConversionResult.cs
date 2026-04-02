namespace ElBruno.MarkItDotNet;

/// <summary>
/// Represents the outcome of a file-to-Markdown conversion.
/// </summary>
public sealed class ConversionResult
{
    /// <summary>The converted Markdown content, or empty on failure.</summary>
    public string Markdown { get; }

    /// <summary>The source format that was converted (e.g., ".txt").</summary>
    public string SourceFormat { get; }

    /// <summary>Whether the conversion succeeded.</summary>
    public bool Success { get; }

    /// <summary>Error message when <see cref="Success"/> is false; null otherwise.</summary>
    public string? ErrorMessage { get; }

    /// <summary>Optional metadata about the conversion (timing, word count, page count).</summary>
    public ConversionMetadata? Metadata { get; }

    private ConversionResult(string markdown, string sourceFormat, bool success, string? errorMessage, ConversionMetadata? metadata)
    {
        Markdown = markdown;
        SourceFormat = sourceFormat;
        Success = success;
        ErrorMessage = errorMessage;
        Metadata = metadata;
    }

    /// <summary>Creates a successful conversion result.</summary>
    public static ConversionResult Succeeded(string markdown, string sourceFormat, ConversionMetadata? metadata = null) =>
        new(markdown, sourceFormat, success: true, errorMessage: null, metadata: metadata);

    /// <summary>Creates a failed conversion result.</summary>
    public static ConversionResult Failure(string error, string sourceFormat) =>
        new(string.Empty, sourceFormat, success: false, errorMessage: error, metadata: null);
}
