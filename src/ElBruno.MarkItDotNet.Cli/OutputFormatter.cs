using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElBruno.MarkItDotNet.Cli;

/// <summary>
/// Formats conversion results for stdout output.
/// </summary>
internal static class OutputFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string FormatMarkdown(ConversionResult result) => result.Markdown;

    public static string FormatJson(ConversionResult result)
    {
        var output = new JsonOutput
        {
            Success = result.Success,
            Markdown = result.Success ? result.Markdown : null,
            Error = result.ErrorMessage,
            SourceFormat = result.SourceFormat,
            Metadata = result.Metadata is not null
                ? new JsonMetadata
                {
                    WordCount = result.Metadata.WordCount,
                    ProcessingTimeMs = Math.Round(result.Metadata.ProcessingTime.TotalMilliseconds, 1)
                }
                : null
        };

        return JsonSerializer.Serialize(output, JsonOptions);
    }

    public static string Format(ConversionResult result, string format)
    {
        return string.Equals(format, "json", StringComparison.OrdinalIgnoreCase)
            ? FormatJson(result)
            : FormatMarkdown(result);
    }

    private sealed class JsonOutput
    {
        public bool Success { get; init; }
        public string? Markdown { get; init; }
        public string? Error { get; init; }
        public string? SourceFormat { get; init; }
        public JsonMetadata? Metadata { get; init; }
    }

    private sealed class JsonMetadata
    {
        public int? WordCount { get; init; }
        public double ProcessingTimeMs { get; init; }
    }
}
