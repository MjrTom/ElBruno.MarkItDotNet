using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  📘 DOCX Conversion Sample                               ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();
var markdownService = sp.GetRequiredService<MarkdownService>();

// Build a DOCX in memory with rich content
var docxBytes = CreateSampleDocx();
Console.WriteLine($"📝 Created in-memory DOCX ({docxBytes.Length} bytes)\n");

Console.WriteLine("📄 Converting DOCX → Markdown");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

using (var stream = new MemoryStream(docxBytes))
{
    var result = await markdownService.ConvertAsync(stream, ".docx");
    if (result.Success)
    {
        Console.WriteLine("✅ DOCX conversion succeeded!");
        Console.WriteLine($"   Format: {result.SourceFormat}");
        if (result.Metadata is not null)
            Console.WriteLine($"   Words: {result.Metadata.WordCount} | Time: {result.Metadata.ProcessingTime.TotalMilliseconds:F1}ms");
        Console.WriteLine($"\n{result.Markdown}");
    }
    else
    {
        Console.WriteLine($"❌ Failed: {result.ErrorMessage}");
    }
}

Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅ DOCX Sample Complete!                                 ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

static byte[] CreateSampleDocx()
{
    using var ms = new MemoryStream();
    using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
    {
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body!;

        // Heading 1
        body.Append(new Paragraph(
            new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }),
            new Run(new Text("MarkItDotNet Overview"))));

        // Normal paragraph with bold and italic
        body.Append(new Paragraph(
            new Run(new Text("MarkItDotNet is a ")),
            new Run(
                new RunProperties(new Bold()),
                new Text("powerful") { Space = SpaceProcessingModeValues.Preserve }),
            new Run(new Text(" and ")),
            new Run(
                new RunProperties(new Italic()),
                new Text("flexible") { Space = SpaceProcessingModeValues.Preserve }),
            new Run(new Text(" .NET library for converting documents to Markdown."))));

        // Heading 2
        body.Append(new Paragraph(
            new ParagraphProperties(new ParagraphStyleId { Val = "Heading2" }),
            new Run(new Text("Supported Formats"))));

        // Bulleted list (unordered)
        var numPart = mainPart.AddNewPart<NumberingDefinitionsPart>();
        numPart.Numbering = new Numbering(
            new AbstractNum(
                new Level(
                    new NumberingFormat { Val = NumberFormatValues.Bullet },
                    new LevelText { Val = "•" })
                { LevelIndex = 0 })
            { AbstractNumberId = 1 },
            new NumberingInstance(new AbstractNumId { Val = 1 }) { NumberID = 1 });

        foreach (var item in new[] { "PDF documents", "Word (DOCX) files", "CSV and TSV data", "XML and YAML configs" })
        {
            body.Append(new Paragraph(
                new ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference { Val = 0 },
                        new NumberingId { Val = 1 })),
                new Run(new Text(item))));
        }

        // Heading 2
        body.Append(new Paragraph(
            new ParagraphProperties(new ParagraphStyleId { Val = "Heading2" }),
            new Run(new Text("Comparison Table"))));

        // Simple table
        var table = new Table(
            new TableRow(
                new TableCell(new Paragraph(new Run(new Text("Format")))),
                new TableCell(new Paragraph(new Run(new Text("Streaming")))),
                new TableCell(new Paragraph(new Run(new Text("Status"))))),
            new TableRow(
                new TableCell(new Paragraph(new Run(new Text("PDF")))),
                new TableCell(new Paragraph(new Run(new Text("Yes")))),
                new TableCell(new Paragraph(new Run(new Text("Stable"))))),
            new TableRow(
                new TableCell(new Paragraph(new Run(new Text("DOCX")))),
                new TableCell(new Paragraph(new Run(new Text("No")))),
                new TableCell(new Paragraph(new Run(new Text("Stable"))))));
        body.Append(table);

        // Hyperlink
        var relId = mainPart.AddHyperlinkRelationship(
            new Uri("https://github.com/elbruno/ElBruno.MarkItDotNet"), true).Id;
        body.Append(new Paragraph(
            new Run(new Text("Visit the project: ")),
            new Hyperlink(
                new Run(
                    new RunProperties(new Color { Val = "0563C1" }),
                    new Text("MarkItDotNet on GitHub")))
            { Id = relId }));
    }

    return ms.ToArray();
}
