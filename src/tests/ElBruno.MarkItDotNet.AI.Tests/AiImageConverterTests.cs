using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.AI.Tests;

public class AiImageConverterTests
{
    private readonly AiImageConverter _converter = new(new TestChatClient("# Image Description\n\nA photo of a cat."), new AiOptions());

    [Theory]
    [InlineData(".png")]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".gif")]
    [InlineData(".bmp")]
    [InlineData(".webp")]
    public void CanHandle_SupportedExtensions_ReturnsTrue(string extension)
    {
        _converter.CanHandle(extension).Should().BeTrue();
    }

    [Theory]
    [InlineData(".pdf")]
    [InlineData(".txt")]
    [InlineData(".svg")]
    [InlineData(".mp3")]
    public void CanHandle_UnsupportedExtensions_ReturnsFalse(string extension)
    {
        _converter.CanHandle(extension).Should().BeFalse();
    }

    [Fact]
    public async Task ConvertAsync_WithMockClient_ReturnsAiResponse()
    {
        using var stream = new MemoryStream(new byte[] { 0x89, 0x50, 0x4E, 0x47 }); // PNG magic bytes
        var result = await _converter.ConvertAsync(stream, ".png");

        result.Should().Contain("Image Description");
        result.Should().Contain("cat");
    }

    [Fact]
    public async Task ConvertAsync_NullStream_ThrowsArgumentNullException()
    {
        var act = () => _converter.ConvertAsync(null!, ".png");
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
