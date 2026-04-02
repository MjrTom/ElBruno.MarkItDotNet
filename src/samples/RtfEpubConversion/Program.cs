using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ElBruno.MarkItDotNet - RTF & EPub Conversion Sample     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var serviceProvider = services.BuildServiceProvider();
var markdownService = serviceProvider.GetRequiredService<MarkdownService>();

// ── RTF to Markdown ──────────────────────────────────────────────────────
Console.WriteLine("📄 RTF to Markdown Conversion");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var rtfContent = @"{\rtf1\ansi\deff0
{\fonttbl{\f0 Arial;}}
{\b Welcome to MarkItDotNet!}\par
\par
This library converts many file formats to Markdown:\par
{\b\i RTF}, HTML, DOCX, PDF, Excel, and more.\par
\par
Perfect for {\ul AI pipelines} and document processing.
}";

using (var rtfStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rtfContent)))
{
    var result = await markdownService.ConvertAsync(rtfStream, ".rtf");
    if (result.Success)
    {
        Console.WriteLine("✅ RTF conversion succeeded!");
        Console.WriteLine($"   Source format: {result.SourceFormat}\n");
        Console.WriteLine("Converted Markdown:");
        Console.WriteLine(result.Markdown);
    }
    else
    {
        Console.WriteLine($"❌ Conversion failed: {result.ErrorMessage}");
    }
}

// ── EPub format check ────────────────────────────────────────────────────
Console.WriteLine("\n📚 EPub Format Support Check");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var registry = serviceProvider.GetRequiredService<ConverterRegistry>();
var canHandleEpub = registry.Resolve(".epub") is not null;

Console.WriteLine($"✅ EPub (.epub) converter registered: {canHandleEpub}");
Console.WriteLine("   EPub files require a valid .epub archive (ZIP with XHTML chapters).");
Console.WriteLine("   Creating one in-memory is complex, so this demo just verifies registration.");
Console.WriteLine("   To convert a real EPub file, pass its stream to MarkdownService.ConvertAsync().\n");

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║             RTF/EPub Sample Complete!                    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
