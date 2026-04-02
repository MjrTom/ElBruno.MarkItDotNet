using System.Text;
using ElBruno.MarkItDotNet;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MarkItDotNet - Plugin Package Sample                      ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
var sp = services.BuildServiceProvider();

var registry = sp.GetRequiredService<ConverterRegistry>();

// Register our custom plugin that bundles .env and .properties converters
registry.RegisterPlugin(new ConfigFilesPlugin());

Console.WriteLine($"🔌 Registered plugin: ConfigFilesPlugin");
Console.WriteLine($"   Plugins loaded: {registry.GetPlugins().Count}\n");

var markdownService = sp.GetRequiredService<MarkdownService>();

// --- Test .env conversion ---
var envContent = """
    DATABASE_URL=postgres://localhost:5432/mydb
    API_KEY=sk-demo-key-12345
    NODE_ENV=production
    LOG_LEVEL=debug
    PORT=3000
    """;

Console.WriteLine("📝 Converting .env file:");
Console.WriteLine("────────────────────────────────────────");
using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(envContent)))
{
    var result = await markdownService.ConvertAsync(stream, ".env");
    Console.WriteLine(result.Success ? result.Markdown : $"❌ {result.ErrorMessage}");
}

// --- Test .properties conversion ---
var propsContent = """
    app.name=MyApplication
    app.version=2.1.0
    db.host=localhost
    db.port=3306
    db.name=app_database
    """;

Console.WriteLine("📝 Converting .properties file:");
Console.WriteLine("────────────────────────────────────────");
using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(propsContent)))
{
    var result = await markdownService.ConvertAsync(stream, ".properties");
    Console.WriteLine(result.Success ? result.Markdown : $"❌ {result.ErrorMessage}");
}

Console.WriteLine("💡 Tip: Use IConverterPlugin to bundle multiple converters into a single package.");

// --- Plugin bundles two converters ---
sealed class ConfigFilesPlugin : IConverterPlugin
{
    public string Name => "ConfigFilesPlugin";
    public IEnumerable<IMarkdownConverter> GetConverters() =>
        [new EnvConverter(), new PropertiesConverter()];
}

sealed class EnvConverter : IMarkdownConverter
{
    public bool CanHandle(string ext) => ext.Equals(".env", StringComparison.OrdinalIgnoreCase);

    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken ct = default)
    {
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var sb = new StringBuilder("# Environment Variables\n\n| Variable | Value |\n|----------|-------|\n");
        foreach (var raw in reader.ReadToEnd().Split('\n'))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
                sb.AppendLine($"| `{parts[0].Trim()}` | `{parts[1].Trim()}` |");
        }
        return Task.FromResult(sb.ToString());
    }
}

sealed class PropertiesConverter : IMarkdownConverter
{
    public bool CanHandle(string ext) => ext.Equals(".properties", StringComparison.OrdinalIgnoreCase);

    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken ct = default)
    {
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var sb = new StringBuilder("# Properties File\n\n| Property | Value |\n|----------|-------|\n");
        foreach (var raw in reader.ReadToEnd().Split('\n'))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#') || line.StartsWith('!')) continue;
            var sep = line.IndexOfAny(['=', ':']);
            if (sep > 0)
                sb.AppendLine($"| `{line[..sep].Trim()}` | `{line[(sep + 1)..].Trim()}` |");
        }
        return Task.FromResult(sb.ToString());
    }
}
