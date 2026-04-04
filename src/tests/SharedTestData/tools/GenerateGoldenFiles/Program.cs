using ElBruno.MarkItDotNet;
using ElBruno.MarkItDotNet.Excel;
using ElBruno.MarkItDotNet.PowerPoint;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMarkItDotNet();
services.AddMarkItDotNetExcel();
services.AddMarkItDotNetPowerPoint();
var sp = services.BuildServiceProvider();
var markdownService = sp.GetRequiredService<MarkdownService>();

var documentsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "documents"));
var outputDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "expected", "markitdotnet"));
Directory.CreateDirectory(outputDir);

Console.WriteLine($"Documents: {documentsDir}");
Console.WriteLine($"Output:    {outputDir}");
Console.WriteLine();

var files = Directory.GetFiles(documentsDir)
    .OrderBy(f => f)
    .ToArray();

foreach (var file in files)
{
    var fileName = Path.GetFileName(file);
    var ext = Path.GetExtension(file).ToLowerInvariant();
    var outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + ext.Replace(".", "_") + ".md");

    try
    {
        using var stream = File.OpenRead(file);
        var result = await markdownService.ConvertAsync(stream, ext);
        await File.WriteAllTextAsync(outputFile, result.Markdown);
        Console.WriteLine($"  OK: {fileName} -> {Path.GetFileName(outputFile)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL: {fileName} -> {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("Golden file generation complete.");
