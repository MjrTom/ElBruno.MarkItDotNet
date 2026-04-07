using System.Diagnostics;

namespace ElBruno.MarkItDotNet.Cli.Tests;

/// <summary>
/// Helper to invoke the CLI via <c>dotnet run</c> and capture stdout, stderr, and exit code.
/// </summary>
internal sealed class CliRunner
{
    private static readonly string ProjectPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "ElBruno.MarkItDotNet.Cli", "ElBruno.MarkItDotNet.Cli.csproj"));

    public string Stdout { get; private set; } = string.Empty;
    public string Stderr { get; private set; } = string.Empty;
    public int ExitCode { get; private set; }

    public async Task RunAsync(string arguments, int timeoutMs = 60_000)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{ProjectPath}\" -p:TargetFrameworks=net8.0 --no-build --no-restore -- {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;
        using var cts = new CancellationTokenSource(timeoutMs);

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cts.Token);
        var stderrTask = process.StandardError.ReadToEndAsync(cts.Token);

        await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);

        Stdout = await stdoutTask.ConfigureAwait(false);
        Stderr = await stderrTask.ConfigureAwait(false);
        ExitCode = process.ExitCode;
    }
}
