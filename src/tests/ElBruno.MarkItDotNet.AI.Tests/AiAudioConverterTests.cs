using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.AI.Tests;

public class AiAudioConverterTests
{
    private readonly AiAudioConverter _converter = new(new TestChatClient("## Transcription\n\nHello, world!"), new AiOptions());

    [Theory]
    [InlineData(".wav")]
    [InlineData(".mp3")]
    [InlineData(".m4a")]
    [InlineData(".ogg")]
    [InlineData(".flac")]
    public void CanHandle_SupportedAudioFormats_ReturnsTrue(string extension)
    {
        _converter.CanHandle(extension).Should().BeTrue();
    }

    [Theory]
    [InlineData(".pdf")]
    [InlineData(".png")]
    [InlineData(".txt")]
    [InlineData(".aac")]
    public void CanHandle_UnsupportedFormats_ReturnsFalse(string extension)
    {
        _converter.CanHandle(extension).Should().BeFalse();
    }

    [Fact]
    public async Task ConvertAsync_WithMockClient_ReturnsTranscription()
    {
        using var stream = new MemoryStream(new byte[] { 0x52, 0x49, 0x46, 0x46 }); // WAV-ish bytes
        var result = await _converter.ConvertAsync(stream, ".wav");

        result.Should().Contain("Transcription");
        result.Should().Contain("Hello, world!");
    }

    [Fact]
    public async Task ConvertAsync_NullStream_ThrowsArgumentNullException()
    {
        var act = () => _converter.ConvertAsync(null!, ".wav");
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
