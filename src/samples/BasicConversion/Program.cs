using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ElBruno.MarkItDotNet - Basic Conversion Sample Demo     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

// Set up DI container and register MarkItDotNet services
var services = new ServiceCollection();
services.AddMarkItDotNet();
var serviceProvider = services.BuildServiceProvider();

var markdownService = serviceProvider.GetRequiredService<MarkdownService>();

// ============================================================================
// Demo 1: Direct API - Plain Text Conversion using Stream
// ============================================================================
Console.WriteLine("📝 DEMO 1: Plain Text Conversion (Stream-based API)");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var plainTextContent = """
This is a simple text document.
It contains multiple lines.
MarkItDotNet can convert it to markdown format.

Key benefits:
- Simple integration
- Supports multiple formats
- Perfect for AI pipelines
""";

using (var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(plainTextContent)))
{
    var textResult = await markdownService.ConvertAsync(textStream, ".txt");
    
    if (textResult.Success)
    {
        Console.WriteLine("✅ Conversion succeeded!");
        Console.WriteLine($"Source format: {textResult.SourceFormat}\n");
        Console.WriteLine("Converted Markdown:");
        Console.WriteLine(textResult.Markdown);
    }
    else
    {
        Console.WriteLine($"❌ Conversion failed: {textResult.ErrorMessage}");
    }
}

Console.WriteLine("\n");

// ============================================================================
// Demo 2: JSON Conversion using Stream
// ============================================================================
Console.WriteLine("📋 DEMO 2: JSON Conversion (Stream-based API)");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var jsonContent = """
{
  "library": "ElBruno.MarkItDotNet",
  "version": "0.1.0",
  "description": ".NET library that converts file formats to Markdown",
  "features": [
    "PDF to Markdown",
    "HTML to Markdown",
    "DOCX to Markdown",
    "Plain text formatting",
    "Image text extraction"
  ],
  "repository": "https://github.com/elbruno/ElBruno.MarkItDotNet"
}
""";

using (var jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent)))
{
    var jsonResult = await markdownService.ConvertAsync(jsonStream, ".json");
    
    if (jsonResult.Success)
    {
        Console.WriteLine("✅ Conversion succeeded!");
        Console.WriteLine($"Source format: {jsonResult.SourceFormat}\n");
        Console.WriteLine("Converted Markdown:");
        Console.WriteLine(jsonResult.Markdown);
    }
    else
    {
        Console.WriteLine($"❌ Conversion failed: {jsonResult.ErrorMessage}");
    }
}

Console.WriteLine("\n");

// ============================================================================
// Demo 3: HTML Conversion using Stream
// ============================================================================
Console.WriteLine("🌐 DEMO 3: HTML Conversion (Stream-based API)");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var htmlContent = """
<!DOCTYPE html>
<html>
<head>
    <title>MarkItDotNet Example</title>
</head>
<body>
    <h1>Welcome to MarkItDotNet</h1>
    <p>This is a sample <strong>HTML document</strong> that will be converted to Markdown.</p>
    <ul>
        <li>Fast conversion</li>
        <li>Multiple format support</li>
        <li>AI-ready output</li>
    </ul>
    <p>Learn more at <a href="https://github.com/elbruno/ElBruno.MarkItDotNet">GitHub</a></p>
</body>
</html>
""";

using (var htmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent)))
{
    var htmlResult = await markdownService.ConvertAsync(htmlStream, ".html");
    
    if (htmlResult.Success)
    {
        Console.WriteLine("✅ Conversion succeeded!");
        Console.WriteLine($"Source format: {htmlResult.SourceFormat}\n");
        Console.WriteLine("Converted Markdown:");
        Console.WriteLine(htmlResult.Markdown);
    }
    else
    {
        Console.WriteLine($"❌ Conversion failed: {htmlResult.ErrorMessage}");
    }
}

Console.WriteLine("\n");

// ============================================================================
// Demo 4: Showing Error Handling - Unsupported Format
// ============================================================================
Console.WriteLine("⚠️  DEMO 4: Error Handling (Unsupported Format)");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

var unsupportedContent = "Some binary data";
using (var unsupportedStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(unsupportedContent)))
{
    var unsupportedResult = await markdownService.ConvertAsync(unsupportedStream, ".xyz");
    
    if (unsupportedResult.Success)
    {
        Console.WriteLine("✅ Conversion succeeded!");
        Console.WriteLine(unsupportedResult.Markdown);
    }
    else
    {
        Console.WriteLine($"❌ Conversion failed (as expected)");
        Console.WriteLine($"Error: {unsupportedResult.ErrorMessage}");
        Console.WriteLine($"Source format: {unsupportedResult.SourceFormat}");
    }
}

Console.WriteLine("\n");

// ============================================================================
// Demo 5: Using Dependency Injection Directly
// ============================================================================
Console.WriteLine("🔧 DEMO 5: Using DI Container (Advanced Pattern)");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

Console.WriteLine("✅ MarkItDotNet services registered via AddMarkItDotNet()");
Console.WriteLine("   Services available in the container:");
Console.WriteLine("   - MarkdownService: Main conversion service");
Console.WriteLine("   - ConverterRegistry: Manages format converters");
Console.WriteLine("   - MarkItDotNetOptions: Configuration options");
Console.WriteLine("   - Built-in converters: PlainText, JSON, HTML, DOCX, PDF, Image\n");

var registry = serviceProvider.GetRequiredService<ConverterRegistry>();
Console.WriteLine($"✅ Converter Registry initialized with {registry.GetType().Name}");
Console.WriteLine("   This registry automatically routes to the appropriate converter");
Console.WriteLine("   based on the file extension.\n");

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║             Sample Demo Complete!                        ║");
Console.WriteLine("║  For more information, visit the GitHub repository:      ║");
Console.WriteLine("║  https://github.com/elbruno/ElBruno.MarkItDotNet        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
