using ElBruno.MarkItDotNet.Converters;
using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Tests;

public class MarkdownServiceTests
{
    private static MarkdownService CreateService()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());
        return new MarkdownService(registry);
    }

    [Fact]
    public async Task ConvertAsync_FilePath_ReturnSuccess()
    {
        var service = CreateService();
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Hello world");
            File.Move(path, Path.ChangeExtension(path, ".txt"), overwrite: true);
            path = Path.ChangeExtension(path, ".txt");

            var result = await service.ConvertAsync(path);

            result.Success.Should().BeTrue();
            result.Markdown.Should().Be("Hello world");
            result.SourceFormat.Should().Be(".txt");
            result.ErrorMessage.Should().BeNull();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task ConvertAsync_Stream_ReturnSuccess()
    {
        var service = CreateService();
        using var stream = new MemoryStream("Stream content"u8.ToArray());

        var result = await service.ConvertAsync(stream, ".txt");

        result.Success.Should().BeTrue();
        result.Markdown.Should().Be("Stream content");
        result.SourceFormat.Should().Be(".txt");
    }

    [Fact]
    public async Task ConvertAsync_UnsupportedFormat_ReturnFailure()
    {
        var service = CreateService();
        using var stream = new MemoryStream(Array.Empty<byte>());

        var result = await service.ConvertAsync(stream, ".xyz");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not supported");
        result.SourceFormat.Should().Be(".xyz");
    }

    [Fact]
    public async Task ConvertAsync_NullFilePath_ThrowsArgumentException()
    {
        var service = CreateService();
        var act = () => service.ConvertAsync((string)null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_NullStream_ThrowsArgumentNullException()
    {
        var service = CreateService();
        var act = () => service.ConvertAsync((Stream)null!, ".txt");
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_NullRegistry_ThrowsArgumentNullException()
    {
        var act = () => new MarkdownService(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task ConvertAsync_EmptyFilePath_ThrowsArgumentException()
    {
        var service = CreateService();
        var act = () => service.ConvertAsync(string.Empty);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_WhitespaceFilePath_ThrowsArgumentException()
    {
        var service = CreateService();
        var act = () => service.ConvertAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_Stream_NullExtension_ThrowsArgumentException()
    {
        var service = CreateService();
        using var stream = new MemoryStream();
        var act = () => service.ConvertAsync(stream, null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_Stream_EmptyExtension_ThrowsArgumentException()
    {
        var service = CreateService();
        using var stream = new MemoryStream();
        var act = () => service.ConvertAsync(stream, string.Empty);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_UnsupportedFilePath_ReturnFailure()
    {
        var service = CreateService();
        var path = Path.GetTempFileName();
        try
        {
            File.Move(path, Path.ChangeExtension(path, ".xyz"), overwrite: true);
            path = Path.ChangeExtension(path, ".xyz");

            var result = await service.ConvertAsync(path);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not supported");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task ConvertAsync_Stream_CaseInsensitiveExtension()
    {
        var service = CreateService();
        using var stream = new MemoryStream("content"u8.ToArray());

        var result = await service.ConvertAsync(stream, ".TXT");

        result.Success.Should().BeTrue();
        result.Markdown.Should().Be("content");
    }

    [Fact]
    public async Task ConvertAsync_FileNotFound_ReturnFailure()
    {
        var service = CreateService();

        var result = await service.ConvertAsync("nonexistent_file.txt");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }
}
