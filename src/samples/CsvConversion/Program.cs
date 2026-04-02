using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  📊 CSV & TSV Conversion Sample                         ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();
var markdownService = sp.GetRequiredService<MarkdownService>();

// ── Demo 1: CSV to Markdown Table ──────────────────────────────────────
Console.WriteLine("📄 DEMO 1: CSV → Markdown Table");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var csv = """
    Name,Language,Stars,Category
    MarkItDotNet,.NET,42,Conversion
    PdfPig,.NET,1200,PDF
    Markdig,.NET,4300,Markdown
    """;

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv)))
{
    var result = await markdownService.ConvertAsync(stream, ".csv");
    if (result.Success)
    {
        Console.WriteLine("✅ CSV conversion succeeded!");
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

Console.WriteLine("\n");

// ── Demo 2: TSV to Markdown Table ──────────────────────────────────────
Console.WriteLine("📋 DEMO 2: TSV → Markdown Table");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var tsv = "Product\tPrice\tIn Stock\n" +
          "Widget A\t$9.99\tYes\n" +
          "Widget B\t$14.50\tNo\n" +
          "Widget C\t$3.25\tYes";

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(tsv)))
{
    var result = await markdownService.ConvertAsync(stream, ".tsv");
    if (result.Success)
    {
        Console.WriteLine("✅ TSV conversion succeeded!");
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

Console.WriteLine("\n");

// ── Demo 3: CSV with quoted fields ─────────────────────────────────────
Console.WriteLine("🔤 DEMO 3: CSV with Quoted Fields");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var quotedCsv = """
    City,Country,Population,Notes
    "New York",USA,"8,336,817","Largest U.S. city"
    Tokyo,Japan,"13,960,000","Capital of Japan"
    "São Paulo",Brazil,"12,325,232","Largest in South America"
    """;

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(quotedCsv)))
{
    var result = await markdownService.ConvertAsync(stream, ".csv");
    if (result.Success)
    {
        Console.WriteLine("✅ Quoted CSV conversion succeeded!");
        Console.WriteLine($"\n{result.Markdown}");
    }
    else
    {
        Console.WriteLine($"❌ Failed: {result.ErrorMessage}");
    }
}

Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅ CSV/TSV Sample Complete!                             ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
