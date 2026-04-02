namespace ElBruno.MarkItDotNet;

/// <summary>
/// Metadata about a completed conversion, including timing and content statistics.
/// </summary>
public class ConversionMetadata
{
    /// <summary>Number of pages in the source document (PDF, DOCX, etc.), or null if not applicable.</summary>
    public int? PageCount { get; init; }

    /// <summary>Word count of the resulting Markdown content.</summary>
    public int? WordCount { get; init; }

    /// <summary>Wall-clock time spent performing the conversion.</summary>
    public TimeSpan ProcessingTime { get; init; }
}
