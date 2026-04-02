using Microsoft.Extensions.AI;

namespace ElBruno.MarkItDotNet.AI;

/// <summary>
/// Plugin that registers all AI-powered converters.
/// </summary>
public class AiConverterPlugin : IConverterPlugin
{
    /// <inheritdoc />
    public string Name => "AI";

    private readonly IChatClient _chatClient;
    private readonly AiOptions _options;

    public AiConverterPlugin(IChatClient chatClient, AiOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chatClient);
        _chatClient = chatClient;
        _options = options ?? new AiOptions();
    }

    /// <inheritdoc />
    public IEnumerable<IMarkdownConverter> GetConverters() =>
    [
        new AiImageConverter(_chatClient, _options),
        new AiPdfConverter(_chatClient, _options),
        new AiAudioConverter(_chatClient, _options)
    ];
}
