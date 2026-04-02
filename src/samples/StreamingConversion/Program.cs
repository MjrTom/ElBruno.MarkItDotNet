using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;
using UglyToad.PdfPig.Writer;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MarkItDotNet - Streaming Conversion Sample                ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();

var markdownService = sp.GetRequiredService<MarkdownService>();

// Create a 3-page PDF in-memory using PdfPig
Console.WriteLine("📄 Creating a 3-page PDF in-memory...\n");
using var pdfStream = new MemoryStream();
var builder = new PdfDocumentBuilder();

var font = builder.AddStandard14Font(UglyToad.PdfPig.Fonts.Standard14Fonts.Standard14Font.Helvetica);

for (var i = 1; i <= 3; i++)
{
    var page = builder.AddPage(UglyToad.PdfPig.Content.PageSize.A4);
    page.AddText($"Page {i}: MarkItDotNet Streaming Demo", 16, new UglyToad.PdfPig.Core.PdfPoint(50, 750), font);
    page.AddText($"This is paragraph content on page {i}.", 12, new UglyToad.PdfPig.Core.PdfPoint(50, 720), font);
    page.AddText($"Streaming converts large documents chunk by chunk.", 12, new UglyToad.PdfPig.Core.PdfPoint(50, 700), font);
}

var pdfBytes = builder.Build();
pdfStream.Write(pdfBytes);
pdfStream.Position = 0;

Console.WriteLine("🔄 Streaming conversion (IAsyncEnumerable<string>):\n");
Console.WriteLine("── Chunks ──────────────────────────────────────────────────");

var chunkIndex = 0;
await foreach (var chunk in markdownService.ConvertStreamingAsync(pdfStream, ".pdf"))
{
    chunkIndex++;
    Console.WriteLine($"\n[Chunk {chunkIndex}]");
    Console.WriteLine("────────────────────────────────────────");

    // Show first 200 chars of each chunk to keep output readable
    var preview = chunk.Length > 200 ? chunk[..200] + "..." : chunk;
    Console.WriteLine(preview);
}

Console.WriteLine($"\n✅ Streaming complete! Received {chunkIndex} chunk(s).");
Console.WriteLine("\n💡 Tip: Streaming is ideal for large PDFs — process pages as they arrive");
Console.WriteLine("   instead of waiting for the entire document to convert.");
