using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using FluentAssertions;
using Xunit;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace ElBruno.MarkItDotNet.PowerPoint.Tests;

public class PowerPointConverterTests
{
    private readonly PowerPointConverter _converter = new();

    [Fact]
    public void CanHandle_Pptx_ReturnsTrue()
    {
        _converter.CanHandle(".pptx").Should().BeTrue();
    }

    [Theory]
    [InlineData(".docx")]
    [InlineData(".pdf")]
    [InlineData(".xlsx")]
    [InlineData(".txt")]
    [InlineData(".ppt")]
    public void CanHandle_Other_ReturnsFalse(string extension)
    {
        _converter.CanHandle(extension).Should().BeFalse();
    }

    [Fact]
    public async Task ConvertAsync_SimplePresentation_ReturnsSlideContent()
    {
        using var stream = CreatePresentationStream(new[]
        {
            new SlideContent("Hello World", null)
        });

        var result = await _converter.ConvertAsync(stream, ".pptx");

        result.Should().Contain("## Slide 1");
        result.Should().Contain("Hello World");
    }

    [Fact]
    public async Task ConvertAsync_WithSpeakerNotes_IncludesNotes()
    {
        using var stream = CreatePresentationStream(new[]
        {
            new SlideContent("Slide Title", "These are my speaker notes")
        });

        var result = await _converter.ConvertAsync(stream, ".pptx");

        result.Should().Contain("## Slide 1");
        result.Should().Contain("Slide Title");
        result.Should().Contain("> **Notes:**");
        result.Should().Contain("These are my speaker notes");
    }

    [Fact]
    public async Task ConvertAsync_MultipleSlides_ReturnsAll()
    {
        using var stream = CreatePresentationStream(new[]
        {
            new SlideContent("First Slide", null),
            new SlideContent("Second Slide", null),
            new SlideContent("Third Slide", null)
        });

        var result = await _converter.ConvertAsync(stream, ".pptx");

        result.Should().Contain("## Slide 1");
        result.Should().Contain("First Slide");
        result.Should().Contain("## Slide 2");
        result.Should().Contain("Second Slide");
        result.Should().Contain("## Slide 3");
        result.Should().Contain("Third Slide");
    }

    [Fact]
    public void Plugin_RegistersConverter()
    {
        var plugin = new PowerPointPlugin();

        plugin.Name.Should().Be("PowerPoint");
        plugin.GetConverters().Should().ContainSingle()
            .Which.Should().BeOfType<PowerPointConverter>();
    }

    private record SlideContent(string Text, string? Notes);

    private static MemoryStream CreatePresentationStream(SlideContent[] slides)
    {
        var ms = new MemoryStream();

        using (var presentationDocument = PresentationDocument.Create(ms, PresentationDocumentType.Presentation))
        {
            var presentationPart = presentationDocument.AddPresentationPart();
            presentationPart.Presentation = new Presentation();
            var slideIdList = new SlideIdList();
            presentationPart.Presentation.SlideIdList = slideIdList;

            uint slideId = 256;

            foreach (var slideContent in slides)
            {
                var slidePart = presentationPart.AddNewPart<SlidePart>();

                slidePart.Slide = new Slide(
                    new CommonSlideData(
                        new ShapeTree(
                            new P.NonVisualGroupShapeProperties(
                                new P.NonVisualDrawingProperties { Id = 1U, Name = "" },
                                new P.NonVisualGroupShapeDrawingProperties(),
                                new ApplicationNonVisualDrawingProperties()),
                            new GroupShapeProperties(new A.TransformGroup()),
                            new P.Shape(
                                new P.NonVisualShapeProperties(
                                    new P.NonVisualDrawingProperties { Id = 2U, Name = "Title" },
                                    new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true }),
                                    new ApplicationNonVisualDrawingProperties()),
                                new P.ShapeProperties(),
                                new P.TextBody(
                                    new A.BodyProperties(),
                                    new A.ListStyle(),
                                    new A.Paragraph(
                                        new A.Run(
                                            new A.RunProperties { Language = "en-US" },
                                            new A.Text { Text = slideContent.Text })))))));

                // Add speaker notes if provided
                if (slideContent.Notes is not null)
                {
                    var notesSlidePart = slidePart.AddNewPart<NotesSlidePart>();
                    notesSlidePart.NotesSlide = new NotesSlide(
                        new CommonSlideData(
                            new ShapeTree(
                                new P.NonVisualGroupShapeProperties(
                                    new P.NonVisualDrawingProperties { Id = 1U, Name = "" },
                                    new P.NonVisualGroupShapeDrawingProperties(),
                                    new ApplicationNonVisualDrawingProperties()),
                                new GroupShapeProperties(new A.TransformGroup()),
                                new P.Shape(
                                    new P.NonVisualShapeProperties(
                                        new P.NonVisualDrawingProperties { Id = 2U, Name = "Notes" },
                                        new P.NonVisualShapeDrawingProperties(),
                                        new ApplicationNonVisualDrawingProperties()),
                                    new P.ShapeProperties(),
                                    new P.TextBody(
                                        new A.BodyProperties(),
                                        new A.ListStyle(),
                                        new A.Paragraph(
                                            new A.Run(
                                                new A.RunProperties { Language = "en-US" },
                                                new A.Text { Text = slideContent.Notes })))))));
                }

                var relId = presentationPart.GetIdOfPart(slidePart);
                slideIdList.Append(new SlideId { Id = slideId++, RelationshipId = relId });
            }

            presentationPart.Presentation.Save();
        }

        ms.Position = 0;
        return ms;
    }
}
