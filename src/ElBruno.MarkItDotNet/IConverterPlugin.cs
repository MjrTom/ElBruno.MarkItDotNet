namespace ElBruno.MarkItDotNet;

/// <summary>
/// Contract for satellite packages that bundle one or more <see cref="IMarkdownConverter"/> implementations.
/// Plugins are registered via <see cref="ConverterRegistry.RegisterPlugin"/>.
/// </summary>
public interface IConverterPlugin
{
    /// <summary>
    /// Human-readable name of the plugin (e.g., "OcrPlugin").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns all converters provided by this plugin.
    /// </summary>
    IEnumerable<IMarkdownConverter> GetConverters();
}
