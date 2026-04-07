using System.Text.Json;
using ElBruno.MarkItDotNet.Converters;
using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Cli.Tests;

/// <summary>
/// Tests for OutputFormatter behavior by invoking the CLI with --format options
/// and verifying the structured output.
/// </summary>
public class OutputFormatterTests : IDisposable
{
    private readonly string _tempDir;

    public OutputFormatterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"markitdown-fmt-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private string CreateTempFile(string name, string content)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public async Task JsonOutput_ContainsExpectedFields()
    {
        var filePath = CreateTempFile("test.txt", "Test content for JSON");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" --format json -q");

        cli.ExitCode.Should().Be(0);

        using var doc = JsonDocument.Parse(cli.Stdout);
        var root = doc.RootElement;

        root.TryGetProperty("success", out _).Should().BeTrue("JSON should contain 'success'");
        root.TryGetProperty("markdown", out _).Should().BeTrue("JSON should contain 'markdown'");
        root.TryGetProperty("sourceFormat", out _).Should().BeTrue("JSON should contain 'sourceFormat'");
        root.TryGetProperty("metadata", out _).Should().BeTrue("JSON should contain 'metadata'");
    }

    [Fact]
    public async Task JsonOutput_IsValidJson()
    {
        var filePath = CreateTempFile("valid.txt", "Checking JSON validity");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" --format json -q");

        cli.ExitCode.Should().Be(0);

        var parseAction = () => JsonDocument.Parse(cli.Stdout);
        parseAction.Should().NotThrow("output with --format json must be valid JSON");
    }

    [Fact]
    public async Task JsonOutput_MetadataHasExpectedFields()
    {
        var filePath = CreateTempFile("meta.txt", "Some words here");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" --format json -q");

        cli.ExitCode.Should().Be(0);

        using var doc = JsonDocument.Parse(cli.Stdout);
        var metadata = doc.RootElement.GetProperty("metadata");

        metadata.TryGetProperty("wordCount", out _).Should().BeTrue("metadata should contain 'wordCount'");
        metadata.TryGetProperty("processingTimeMs", out _).Should().BeTrue("metadata should contain 'processingTimeMs'");
    }

    [Fact]
    public async Task MarkdownOutput_IsRawContent()
    {
        var filePath = CreateTempFile("raw.txt", "Raw markdown content");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" --format markdown -q");

        cli.ExitCode.Should().Be(0);
        cli.Stdout.Should().Be("Raw markdown content");
    }

    [Fact]
    public void DirectLibrary_JsonOutput_MatchesExpectedStructure()
    {
        // Test the library ConversionResult/Metadata directly (without CLI)
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());
        var service = new MarkdownService(registry);

        var result = ConversionResult.Succeeded(
            "Test markdown",
            ".txt",
            new ConversionMetadata { WordCount = 2, ProcessingTime = TimeSpan.FromMilliseconds(42) });

        result.Success.Should().BeTrue();
        result.Markdown.Should().Be("Test markdown");
        result.SourceFormat.Should().Be(".txt");
        result.Metadata.Should().NotBeNull();
        result.Metadata!.WordCount.Should().Be(2);
    }
}
