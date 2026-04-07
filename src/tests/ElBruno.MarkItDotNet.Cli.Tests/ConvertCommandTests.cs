using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Cli.Tests;

public class ConvertCommandTests : IDisposable
{
    private readonly string _tempDir;

    public ConvertCommandTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"markitdown-tests-{Guid.NewGuid():N}");
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
    public async Task Convert_PlainTextFile_WritesToStdout()
    {
        var filePath = CreateTempFile("hello.txt", "Hello world");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" -q");

        cli.ExitCode.Should().Be(0);
        cli.Stdout.Should().Contain("Hello world");
    }

    [Fact]
    public async Task Convert_PlainTextFile_WritesToOutputFile()
    {
        var filePath = CreateTempFile("hello.txt", "Hello world");
        var outputPath = Path.Combine(_tempDir, "output.md");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" -o \"{outputPath}\" -q");

        cli.ExitCode.Should().Be(0);
        File.Exists(outputPath).Should().BeTrue();
        var content = await File.ReadAllTextAsync(outputPath);
        content.Should().Contain("Hello world");
    }

    [Fact]
    public async Task Convert_JsonFile_ReturnsMarkdown()
    {
        var json = """{"name":"test","value":42}""";
        var filePath = CreateTempFile("data.json", json);
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" -q");

        cli.ExitCode.Should().Be(0);
        cli.Stdout.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Convert_WithFormatJson_ProducesValidJson()
    {
        var filePath = CreateTempFile("hello.txt", "Hello world");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" --format json -q");

        cli.ExitCode.Should().Be(0);

        // Verify it's valid JSON
        var act = () => JsonDocument.Parse(cli.Stdout);
        act.Should().NotThrow();

        using var doc = JsonDocument.Parse(cli.Stdout);
        doc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        doc.RootElement.GetProperty("markdown").GetString().Should().Contain("Hello world");
        doc.RootElement.GetProperty("sourceFormat").GetString().Should().Be(".txt");
    }

    [Fact]
    public async Task Convert_WithVerbose_ShowsMetadata()
    {
        var filePath = CreateTempFile("hello.txt", "Hello world");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" -v -q");

        cli.ExitCode.Should().Be(0);
        // Verbose metadata goes to stderr
        cli.Stderr.Should().Contain("Format:");
        cli.Stderr.Should().Contain("Words:");
        cli.Stderr.Should().Contain("Time:");
    }

    [Fact]
    public async Task Convert_FileNotFound_ReturnsExitCode2()
    {
        var missingPath = Path.Combine(_tempDir, "nonexistent.txt");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{missingPath}\" -q");

        cli.ExitCode.Should().Be(2);
        cli.Stderr.Should().Contain("file not found");
    }

    [Fact]
    public async Task Convert_UnsupportedFormat_ReturnsExitCode3()
    {
        var filePath = CreateTempFile("data.xyz", "some content");
        var cli = new CliRunner();

        await cli.RunAsync($"\"{filePath}\" -q");

        cli.ExitCode.Should().Be(3);
        cli.Stderr.Should().Contain("not supported");
    }
}
