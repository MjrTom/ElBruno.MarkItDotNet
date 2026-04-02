using System.Text;
using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MarkItDotNet - Custom Converter Sample (.ini files)       ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();

// Register custom INI converter directly with the registry
var registry = sp.GetRequiredService<ConverterRegistry>();
registry.Register(new IniConverter());

var markdownService = sp.GetRequiredService<MarkdownService>();

var iniContent = """
    [database]
    host=localhost
    port=5432
    name=myapp_db

    [server]
    host=0.0.0.0
    port=8080
    workers=4

    [logging]
    level=info
    file=/var/log/app.log
    """;

Console.WriteLine("📝 Input .ini file:");
Console.WriteLine("────────────────────────────────────────");
Console.WriteLine(iniContent);

using var stream = new MemoryStream(Encoding.UTF8.GetBytes(iniContent));
var result = await markdownService.ConvertAsync(stream, ".ini");

if (result.Success)
{
    Console.WriteLine("\n✅ Custom .ini conversion succeeded!");
    Console.WriteLine($"   Words: {result.Metadata?.WordCount}\n");
    Console.WriteLine("── Markdown Output ──────────────────────────────────────");
    Console.WriteLine(result.Markdown);
}
else
{
    Console.WriteLine($"\n❌ Conversion failed: {result.ErrorMessage}");
}

Console.WriteLine("💡 Tip: Implement IMarkdownConverter and call Registry.Register() to add any format.");

// --- Custom INI → Markdown converter ---
sealed class IniConverter : IMarkdownConverter
{
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".ini", StringComparison.OrdinalIgnoreCase);

    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var text = reader.ReadToEnd();

        var sb = new StringBuilder();
        sb.AppendLine("# Configuration File\n");

        string? currentSection = null;
        var rows = new List<(string Key, string Value)>();

        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#'))
                continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                FlushSection(sb, currentSection, rows);
                currentSection = line[1..^1];
                rows.Clear();
            }
            else if (line.Contains('='))
            {
                var parts = line.Split('=', 2);
                rows.Add((parts[0].Trim(), parts[1].Trim()));
            }
        }

        FlushSection(sb, currentSection, rows);
        return Task.FromResult(sb.ToString());
    }

    private static void FlushSection(StringBuilder sb, string? section, List<(string Key, string Value)> rows)
    {
        if (section is null || rows.Count == 0) return;

        sb.AppendLine($"## {section}\n");
        sb.AppendLine("| Key | Value |");
        sb.AppendLine("|-----|-------|");
        foreach (var (key, value) in rows)
            sb.AppendLine($"| `{key}` | `{value}` |");
        sb.AppendLine();
    }
}
