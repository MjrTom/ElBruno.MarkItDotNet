using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Tests;

public class ConversionResultTests
{
    [Fact]
    public void Succeeded_ReturnsSuccessResult()
    {
        var result = ConversionResult.Succeeded("# Title", ".md");

        result.Success.Should().BeTrue();
        result.Markdown.Should().Be("# Title");
        result.SourceFormat.Should().Be(".md");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        var result = ConversionResult.Failure("Something went wrong", ".pdf");

        result.Success.Should().BeFalse();
        result.Markdown.Should().BeEmpty();
        result.SourceFormat.Should().Be(".pdf");
        result.ErrorMessage.Should().Be("Something went wrong");
    }

    [Fact]
    public void Succeeded_WithEmptyMarkdown_ReturnsSuccessWithEmptyContent()
    {
        var result = ConversionResult.Succeeded(string.Empty, ".txt");

        result.Success.Should().BeTrue();
        result.Markdown.Should().BeEmpty();
        result.ErrorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData(".txt")]
    [InlineData(".pdf")]
    [InlineData(".html")]
    [InlineData(".docx")]
    [InlineData(".json")]
    public void Succeeded_PreservesSourceFormat(string format)
    {
        var result = ConversionResult.Succeeded("content", format);

        result.SourceFormat.Should().Be(format);
    }

    [Theory]
    [InlineData(".txt")]
    [InlineData(".pdf")]
    [InlineData(".html")]
    [InlineData(".docx")]
    [InlineData(".json")]
    public void Failure_PreservesSourceFormat(string format)
    {
        var result = ConversionResult.Failure("error", format);

        result.SourceFormat.Should().Be(format);
    }

    [Fact]
    public void Failure_MarkdownIsAlwaysEmpty()
    {
        var result = ConversionResult.Failure("error", ".txt");

        result.Markdown.Should().BeEmpty();
    }

    [Fact]
    public void Succeeded_WithMultilineMarkdown_PreservesContent()
    {
        var markdown = "# Heading\n\nParagraph\n\n- Item 1\n- Item 2";
        var result = ConversionResult.Succeeded(markdown, ".html");

        result.Markdown.Should().Be(markdown);
    }

    [Fact]
    public void Succeeded_ErrorMessageIsAlwaysNull()
    {
        var result = ConversionResult.Succeeded("content", ".txt");

        result.ErrorMessage.Should().BeNull();
    }
}
