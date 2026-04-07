using ElBruno.MarkItDotNet.Excel;
using ElBruno.MarkItDotNet.PowerPoint;
using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.Cli.Commands;

/// <summary>
/// Handler for: markitdown url &lt;url&gt;
/// </summary>
internal static class UrlCommand
{
    public static async Task<int> HandleAsync(
        string url,
        FileInfo? output,
        string format,
        bool quiet,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Console.Error.WriteLine("Error: URL is required.");
            return 1;
        }

        var service = BuildService();

        if (!quiet)
        {
            Console.Error.WriteLine($"Fetching {url}...");
        }

        var result = await service.ConvertUrlAsync(url, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            Console.Error.WriteLine($"Error: {result.ErrorMessage}");
            return 1;
        }

        var formatted = OutputFormatter.Format(result, format);

        if (output is not null)
        {
            var dir = output.Directory;
            if (dir is not null && !dir.Exists)
            {
                dir.Create();
            }

            await File.WriteAllTextAsync(output.FullName, formatted, cancellationToken).ConfigureAwait(false);

            if (!quiet)
            {
                Console.Error.WriteLine($"Written to {output.FullName}");
            }
        }
        else
        {
            Console.Write(formatted);
        }

        return 0;
    }

    private static MarkdownService BuildService()
    {
        var services = new ServiceCollection();
        services.AddMarkItDotNet();
        services.AddMarkItDotNetExcel();
        services.AddMarkItDotNetPowerPoint();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<MarkdownService>();
    }
}
