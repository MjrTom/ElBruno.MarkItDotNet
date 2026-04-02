using System.Text;
using ElBruno.MarkItDotNet;
using ElBruno.MarkItDotNet.Excel;
using ElBruno.MarkItDotNet.PowerPoint;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MarkItDotNet - All Formats Kitchen Sink Demo              ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
services.AddMarkItDotNetExcel();
services.AddMarkItDotNetPowerPoint();
var sp = services.BuildServiceProvider();

var markdownService = sp.GetRequiredService<MarkdownService>();
var results = new List<(string Format, bool Success, int Words, string Error)>();

// Helper to convert in-memory content
async Task ConvertFormat(string ext, string label, byte[] data)
{
    Console.Write($"  Converting {label,-12} ({ext})...");
    using var stream = new MemoryStream(data);
    var result = await markdownService.ConvertAsync(stream, ext);
    if (result.Success)
    {
        var words = result.Metadata?.WordCount ?? 0;
        results.Add((ext, true, words, string.Empty));
        Console.WriteLine($" ✅ {words} words");
    }
    else
    {
        results.Add((ext, false, 0, result.ErrorMessage ?? "Unknown"));
        Console.WriteLine($" ❌ {result.ErrorMessage}");
    }
}

Console.WriteLine("🔄 Converting all supported text-based formats:\n");

// Plain text
await ConvertFormat(".txt", "Plain Text",
    "Hello from MarkItDotNet!\nThis is a plain text document with multiple lines."u8.ToArray());

// JSON
await ConvertFormat(".json", "JSON",
    """{"name":"MarkItDotNet","version":"0.2.0","features":["PDF","HTML","Excel"]}"""u8.ToArray());

// HTML
await ConvertFormat(".html", "HTML",
    "<html><body><h1>Title</h1><p>A <strong>bold</strong> paragraph.</p><ul><li>Item 1</li></ul></body></html>"u8.ToArray());

// CSV
await ConvertFormat(".csv", "CSV",
    "Name,Language,Stars\nMarkItDotNet,C#,100\nPdfPig,C#,500\nClosedXML,C#,4000"u8.ToArray());

// XML
await ConvertFormat(".xml", "XML",
    "<?xml version=\"1.0\"?><root><item id=\"1\"><name>Widget</name><price>9.99</price></item></root>"u8.ToArray());

// YAML
await ConvertFormat(".yaml", "YAML",
    "name: MarkItDotNet\nversion: 0.2.0\nformats:\n  - txt\n  - json\n  - html"u8.ToArray());

// RTF
var rtfContent = @"{\rtf1\ansi{\fonttbl\f0 Arial;}\f0\fs24 Hello from RTF!\par This is a rich text document.\par}";
await ConvertFormat(".rtf", "RTF", Encoding.ASCII.GetBytes(rtfContent));

// Summary table
Console.WriteLine("\n═══════════════════════════════════════════════════════════");
Console.WriteLine("                    📊 Summary Table");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine($"  {"Format",-12} {"Status",-10} {"Words",-8} {"Notes"}");
Console.WriteLine("  ──────────── ────────── ──────── ─────────────────────");

var passed = 0;
var failed = 0;

foreach (var (format, success, words, error) in results)
{
    var status = success ? "✅ Pass" : "❌ Fail";
    var notes = success ? "" : error;
    Console.WriteLine($"  {format,-12} {status,-10} {words,-8} {notes}");
    if (success) passed++;
    else failed++;
}

Console.WriteLine("  ──────────── ────────── ──────── ─────────────────────");
Console.WriteLine($"  Total: {results.Count} formats | ✅ {passed} passed | ❌ {failed} failed");
Console.WriteLine("\n💡 Add Excel/PowerPoint/AI packages for even more formats!");
