using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.AI.Tests;

public class AiConverterPluginTests
{
    [Fact]
    public void Name_ReturnsAI()
    {
        var plugin = new AiConverterPlugin(new TestChatClient());
        plugin.Name.Should().Be("AI");
    }

    [Fact]
    public void GetConverters_Returns3Converters()
    {
        var plugin = new AiConverterPlugin(new TestChatClient());
        var converters = plugin.GetConverters().ToList();

        converters.Should().HaveCount(3);
        converters.Should().ContainSingle(c => c is AiImageConverter);
        converters.Should().ContainSingle(c => c is AiPdfConverter);
        converters.Should().ContainSingle(c => c is AiAudioConverter);
    }

    [Fact]
    public void Constructor_NullChatClient_ThrowsArgumentNullException()
    {
        var act = () => new AiConverterPlugin(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_NullOptions_UsesDefaults()
    {
        var plugin = new AiConverterPlugin(new TestChatClient(), options: null);
        plugin.GetConverters().Should().HaveCount(3);
    }
}
