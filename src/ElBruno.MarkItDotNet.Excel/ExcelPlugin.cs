namespace ElBruno.MarkItDotNet.Excel;

/// <summary>
/// Plugin that provides the Excel converter to the MarkItDotNet converter registry.
/// </summary>
public class ExcelPlugin : IConverterPlugin
{
    /// <inheritdoc />
    public string Name => "Excel";

    /// <inheritdoc />
    public IEnumerable<IMarkdownConverter> GetConverters() => [new ExcelConverter()];
}
