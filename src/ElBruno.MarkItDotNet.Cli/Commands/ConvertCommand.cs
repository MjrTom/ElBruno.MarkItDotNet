using ElBruno.MarkItDotNet.Excel;
using ElBruno.MarkItDotNet.PowerPoint;
using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.Cli.Commands;

/// <summary>
/// Handler for the root convert command: markitdown &lt;file&gt;
/// </summary>
internal static class ConvertCommand
{
    public static async Task<int> HandleAsync(
        FileInfo file,
        FileInfo? output,
        string format,
        bool streaming,
        bool quiet,
        bool verbose,
        CancellationToken cancellationToken)
    {
        if (!file.Exists)
        {
            Console.Error.WriteLine($"Error: file not found — {file.FullName}");
            return 2;
        }

        var service = BuildService();

        if (streaming)
        {
            return await HandleStreamingAsync(service, file, output, quiet, cancellationToken).ConfigureAwait(false);
        }

        if (!quiet)
        {
            Console.Error.WriteLine($"Converting {file.Name}...");
        }

        var result = await service.ConvertAsync(file.FullName, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            Console.Error.WriteLine($"Error: {result.ErrorMessage}");
            return result.ErrorMessage?.Contains("not supported", StringComparison.OrdinalIgnoreCase) == true ? 3 : 1;
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

        if (verbose && result.Metadata is not null)
        {
            Console.Error.WriteLine($"  Format:    {result.SourceFormat}");
            Console.Error.WriteLine($"  Words:     {result.Metadata.WordCount}");
            Console.Error.WriteLine($"  Time:      {result.Metadata.ProcessingTime.TotalMilliseconds:F1}ms");
        }

        return 0;
    }

    private static async Task<int> HandleStreamingAsync(
        MarkdownService service,
        FileInfo file,
        FileInfo? output,
        bool quiet,
        CancellationToken cancellationToken)
    {
        if (!quiet)
        {
            Console.Error.WriteLine($"Streaming {file.Name}...");
        }

        try
        {
            if (output is not null)
            {
                var dir = output.Directory;
                if (dir is not null && !dir.Exists)
                {
                    dir.Create();
                }

                await using var writer = new StreamWriter(output.FullName);
                await foreach (var chunk in service.ConvertStreamingAsync(file.FullName, cancellationToken).ConfigureAwait(false))
                {
                    await writer.WriteAsync(chunk).ConfigureAwait(false);
                }

                if (!quiet)
                {
                    Console.Error.WriteLine($"Written to {output.FullName}");
                }
            }
            else
            {
                await foreach (var chunk in service.ConvertStreamingAsync(file.FullName, cancellationToken).ConfigureAwait(false))
                {
                    Console.Write(chunk);
                }
            }

            return 0;
        }
        catch (NotSupportedException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 3;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
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
