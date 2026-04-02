namespace ElBruno.MarkItDotNet.PowerPoint;

/// <summary>
/// Plugin that provides the PowerPoint converter to the MarkItDotNet converter registry.
/// </summary>
public class PowerPointPlugin : IConverterPlugin
{
    /// <inheritdoc />
    public string Name => "PowerPoint";

    /// <inheritdoc />
    public IEnumerable<IMarkdownConverter> GetConverters() => [new PowerPointConverter()];
}
