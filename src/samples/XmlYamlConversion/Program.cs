using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  📝 XML & YAML Conversion Sample                        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();
var markdownService = sp.GetRequiredService<MarkdownService>();

// ── Demo 1: XML → Fenced Code Block ───────────────────────────────────
Console.WriteLine("🏷️  DEMO 1: XML → Fenced Code Block");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var xml = """
    <?xml version="1.0" encoding="utf-8"?>
    <appSettings>
      <database>
        <server>localhost</server>
        <port>5432</port>
        <name>markitdotnet_db</name>
      </database>
      <features>
        <feature name="pdf" enabled="true" />
        <feature name="docx" enabled="true" />
        <feature name="csv" enabled="true" />
      </features>
      <logging level="Information" />
    </appSettings>
    """;

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
{
    var result = await markdownService.ConvertAsync(stream, ".xml");
    if (result.Success)
    {
        Console.WriteLine("✅ XML conversion succeeded!");
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

// ── Demo 2: YAML → Fenced Code Block ──────────────────────────────────
Console.WriteLine("📋 DEMO 2: YAML → Fenced Code Block");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var yaml = """
    name: MarkItDotNet
    version: 0.1.0
    description: .NET library that converts file formats to Markdown

    converters:
      - format: pdf
        streaming: true
      - format: docx
        streaming: false
      - format: csv
        streaming: false
      - format: xml
        streaming: false

    settings:
      maxFileSizeMb: 100
      enableCaching: true
      logLevel: Information
    """;

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml)))
{
    var result = await markdownService.ConvertAsync(stream, ".yaml");
    if (result.Success)
    {
        Console.WriteLine("✅ YAML conversion succeeded!");
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

// ── Demo 3: YAML with .yml extension ──────────────────────────────────
Console.WriteLine("🔄 DEMO 3: YAML with .yml Extension");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var yml = """
    ci:
      trigger:
        branches: [main, develop]
      steps:
        - name: Build
          run: dotnet build
        - name: Test
          run: dotnet test
        - name: Pack
          run: dotnet pack
    """;

using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yml)))
{
    var result = await markdownService.ConvertAsync(stream, ".yml");
    if (result.Success)
    {
        Console.WriteLine("✅ .yml conversion succeeded!");
        Console.WriteLine($"   Format: {result.SourceFormat}");
        Console.WriteLine($"\n{result.Markdown}");
    }
    else
    {
        Console.WriteLine($"❌ Failed: {result.ErrorMessage}");
    }
}

Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅ XML/YAML Sample Complete!                            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
