namespace ElBruno.MarkItDotNet.AI;

/// <summary>
/// Options for AI-powered converters.
/// </summary>
public class AiOptions
{
    /// <summary>
    /// Prompt sent to the AI model when processing images.
    /// </summary>
    public string? ImagePrompt { get; set; } = "Describe this image in detail. Extract any visible text using OCR. Return the result as Markdown.";

    /// <summary>
    /// Prompt sent to the AI model when processing audio.
    /// </summary>
    public string? AudioPrompt { get; set; } = "Transcribe this audio content. Return the result as Markdown.";
}
