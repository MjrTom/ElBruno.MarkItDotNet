using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.AI.Tests;

public class AiPdfConverterTests
{
    private readonly AiPdfConverter _converter = new(new TestChatClient(), new AiOptions());

    [Fact]
    public void CanHandle_Pdf_ReturnsTrue()
    {
        _converter.CanHandle(".pdf").Should().BeTrue();
    }

    [Theory]
    [InlineData(".png")]
    [InlineData(".txt")]
    [InlineData(".docx")]
    public void CanHandle_NonPdf_ReturnsFalse(string extension)
    {
        _converter.CanHandle(extension).Should().BeFalse();
    }

    [Fact]
    public void CanHandle_CaseInsensitive()
    {
        _converter.CanHandle(".PDF").Should().BeTrue();
        _converter.CanHandle(".Pdf").Should().BeTrue();
    }
}
