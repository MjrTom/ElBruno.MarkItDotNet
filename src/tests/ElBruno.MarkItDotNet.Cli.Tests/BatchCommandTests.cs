using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Cli.Tests;

public class BatchCommandTests : IDisposable
{
    private readonly string _tempDir;

    public BatchCommandTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"markitdown-batch-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private string CreateInputDir(Dictionary<string, string> files)
    {
        var inputDir = Path.Combine(_tempDir, "input");
        Directory.CreateDirectory(inputDir);

        foreach (var (name, content) in files)
        {
            var fullPath = Path.Combine(inputDir, name);
            var dir = Path.GetDirectoryName(fullPath);
            if (dir is not null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(fullPath, content);
        }

        return inputDir;
    }

    [Fact]
    public async Task Batch_ConvertsMixedFiles()
    {
        var inputDir = CreateInputDir(new Dictionary<string, string>
        {
            ["hello.txt"] = "Hello world",
            ["data.json"] = """{"key":"value"}""",
            ["notes.csv"] = "name,age\nAlice,30"
        });
        var outputDir = Path.Combine(_tempDir, "output");
        var cli = new CliRunner();

        await cli.RunAsync($"batch \"{inputDir}\" -o \"{outputDir}\" -q");

        cli.ExitCode.Should().Be(0);
        Directory.Exists(outputDir).Should().BeTrue();

        // At least the .txt and .json should be converted
        var outputFiles = Directory.GetFiles(outputDir, "*.md");
        outputFiles.Length.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Batch_Recursive_ProcessesSubdirectories()
    {
        var inputDir = CreateInputDir(new Dictionary<string, string>
        {
            ["root.txt"] = "Root file",
            [Path.Combine("sub", "nested.txt")] = "Nested file"
        });
        var outputDir = Path.Combine(_tempDir, "output");
        var cli = new CliRunner();

        await cli.RunAsync($"batch \"{inputDir}\" -o \"{outputDir}\" -r -q");

        cli.ExitCode.Should().Be(0);

        // Should have files from both root and sub
        var outputFiles = Directory.GetFiles(outputDir, "*.md", SearchOption.AllDirectories);
        outputFiles.Length.Should().Be(2);
    }

    [Fact]
    public async Task Batch_WithPattern_FiltersFiles()
    {
        var inputDir = CreateInputDir(new Dictionary<string, string>
        {
            ["hello.txt"] = "Hello",
            ["data.json"] = """{"key":"value"}"""
        });
        var outputDir = Path.Combine(_tempDir, "output");
        var cli = new CliRunner();

        await cli.RunAsync($"batch \"{inputDir}\" -o \"{outputDir}\" --pattern *.txt -q");

        cli.ExitCode.Should().Be(0);

        var outputFiles = Directory.GetFiles(outputDir, "*.md", SearchOption.AllDirectories);
        outputFiles.Length.Should().Be(1);

        var fileName = Path.GetFileNameWithoutExtension(outputFiles[0]);
        fileName.Should().Be("hello");
    }

    [Fact]
    public async Task Batch_CreatesOutputDirectoryIfMissing()
    {
        var inputDir = CreateInputDir(new Dictionary<string, string>
        {
            ["hello.txt"] = "Hello"
        });
        var outputDir = Path.Combine(_tempDir, "brand-new-output");
        Directory.Exists(outputDir).Should().BeFalse("output dir should not exist before batch");

        var cli = new CliRunner();
        await cli.RunAsync($"batch \"{inputDir}\" -o \"{outputDir}\" -q");

        cli.ExitCode.Should().Be(0);
        Directory.Exists(outputDir).Should().BeTrue("batch should create the output directory");
    }
}
