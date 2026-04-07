using ElBruno.MarkItDotNet.Excel;
using ElBruno.MarkItDotNet.PowerPoint;
using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.Cli.Commands;

/// <summary>
/// Handler for: markitdown batch &lt;directory&gt; -o &lt;output&gt;
/// </summary>
internal static class BatchCommand
{
    public static async Task<int> HandleAsync(
        DirectoryInfo directory,
        DirectoryInfo output,
        bool recursive,
        string? pattern,
        int parallel,
        string format,
        bool quiet,
        CancellationToken cancellationToken)
    {
        if (!directory.Exists)
        {
            Console.Error.WriteLine($"Error: directory not found — {directory.FullName}");
            return 2;
        }

        if (!output.Exists)
        {
            output.Create();
        }

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var searchPattern = pattern ?? "*.*";
        var files = directory.GetFiles(searchPattern, searchOption);

        if (files.Length == 0)
        {
            Console.Error.WriteLine("No files matched the pattern.");
            return 0;
        }

        var service = BuildService();
        var registry = BuildRegistry();

        // Filter to files we actually have converters for
        var convertible = files
            .Where(f => registry.Resolve(f.Extension.ToLowerInvariant()) is not null)
            .ToArray();

        if (convertible.Length == 0)
        {
            Console.Error.WriteLine("No files matched a supported format.");
            return 0;
        }

        if (!quiet)
        {
            Console.Error.WriteLine($"Processing {convertible.Length} file(s) with up to {parallel} threads...");
        }

        var succeeded = 0;
        var failed = 0;

        var semaphore = new SemaphoreSlim(parallel);

        var tasks = convertible.Select(async file =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var result = await service.ConvertAsync(file.FullName, cancellationToken).ConfigureAwait(false);
                var ext = string.Equals(format, "json", StringComparison.OrdinalIgnoreCase) ? ".json" : ".md";
                var relativePath = Path.GetRelativePath(directory.FullName, file.FullName);
                var outputPath = Path.Combine(output.FullName, Path.ChangeExtension(relativePath, ext));

                var outputDir = Path.GetDirectoryName(outputPath);
                if (outputDir is not null && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (result.Success)
                {
                    var formatted = OutputFormatter.Format(result, format);
                    await File.WriteAllTextAsync(outputPath, formatted, cancellationToken).ConfigureAwait(false);
                    Interlocked.Increment(ref succeeded);

                    if (!quiet)
                    {
                        Console.Error.WriteLine($"  ✓ {file.Name}");
                    }
                }
                else
                {
                    Interlocked.Increment(ref failed);

                    if (!quiet)
                    {
                        Console.Error.WriteLine($"  ✗ {file.Name}: {result.ErrorMessage}");
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);

        if (!quiet)
        {
            Console.Error.WriteLine($"Done: {succeeded} succeeded, {failed} failed.");
        }

        return failed > 0 ? 1 : 0;
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

    private static ConverterRegistry BuildRegistry()
    {
        var services = new ServiceCollection();
        services.AddMarkItDotNet();
        services.AddMarkItDotNetExcel();
        services.AddMarkItDotNetPowerPoint();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ConverterRegistry>();
    }
}


