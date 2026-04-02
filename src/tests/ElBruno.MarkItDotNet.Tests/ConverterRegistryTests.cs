using ElBruno.MarkItDotNet.Converters;
using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Tests;

public class ConverterRegistryTests
{
    [Fact]
    public void Register_AddsConverterToRegistry()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());
        registry.GetAll().Should().HaveCount(1);
    }

    [Fact]
    public void Register_NullConverter_ThrowsArgumentNullException()
    {
        var registry = new ConverterRegistry();
        var act = () => registry.Register(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Resolve_ReturnsMatchingConverter()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());

        var converter = registry.Resolve(".txt");

        converter.Should().NotBeNull();
        converter.Should().BeOfType<PlainTextConverter>();
    }

    [Fact]
    public void Resolve_UnknownExtension_ReturnsNull()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());

        var converter = registry.Resolve(".xyz");

        converter.Should().BeNull();
    }

    [Fact]
    public void Resolve_IsCaseInsensitive()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());

        registry.Resolve(".TXT").Should().NotBeNull();
        registry.Resolve(".Txt").Should().NotBeNull();
    }

    [Fact]
    public void GetAll_ReturnsAllRegisteredConverters()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());

        var all = registry.GetAll();

        all.Should().HaveCount(1);
        all[0].Should().BeOfType<PlainTextConverter>();
    }

    [Fact]
    public void Register_MultipleConverters_ResolveEach()
    {
        var registry = new ConverterRegistry();
        var textConverter = new PlainTextConverter();
        var stubConverter = new StubConverter(".json");

        registry.Register(textConverter);
        registry.Register(stubConverter);

        registry.Resolve(".txt").Should().BeSameAs(textConverter);
        registry.Resolve(".json").Should().BeSameAs(stubConverter);
    }

    [Fact]
    public void GetAll_MultipleConverters_ReturnsAll()
    {
        var registry = new ConverterRegistry();
        registry.Register(new PlainTextConverter());
        registry.Register(new StubConverter(".json"));
        registry.Register(new StubConverter(".html"));

        registry.GetAll().Should().HaveCount(3);
    }

    [Fact]
    public void Resolve_EmptyRegistry_ReturnsNull()
    {
        var registry = new ConverterRegistry();

        registry.Resolve(".txt").Should().BeNull();
    }

    [Fact]
    public void Resolve_DuplicateConverters_ReturnsFirst()
    {
        var registry = new ConverterRegistry();
        var first = new StubConverter(".txt");
        var second = new StubConverter(".txt");

        registry.Register(first);
        registry.Register(second);

        registry.Resolve(".txt").Should().BeSameAs(first);
    }

    /// <summary>
    /// A simple stub converter for testing registry behavior with multiple extensions.
    /// </summary>
    private sealed class StubConverter : IMarkdownConverter
    {
        private readonly string _extension;

        public StubConverter(string extension) => _extension = extension;

        public bool CanHandle(string fileExtension) =>
            fileExtension.Equals(_extension, StringComparison.OrdinalIgnoreCase);

        public Task<string> ConvertAsync(Stream fileStream, string fileExtension) =>
            Task.FromResult("stub");
    }
}
