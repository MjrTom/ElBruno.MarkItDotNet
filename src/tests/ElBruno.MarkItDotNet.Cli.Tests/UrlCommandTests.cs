using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Cli.Tests;

public class UrlCommandTests
{
    [Fact]
    public async Task Url_MissingArgument_ReturnsNonZero()
    {
        var cli = new CliRunner();
        // Calling 'url' without providing the required URL argument should fail
        await cli.RunAsync("url");

        // System.CommandLine returns non-zero for missing required arguments
        cli.ExitCode.Should().NotBe(0);
    }

    [Fact]
    public async Task Url_InvalidUrl_ExecutesWithoutCrash()
    {
        var cli = new CliRunner();
        await cli.RunAsync("url not-a-valid-url -q");

        // The CLI should handle an invalid URL gracefully without throwing
        // The UrlConverter may return success (with error content) or failure;
        // either way, the process should exit cleanly
        (cli.ExitCode == 0 || cli.ExitCode == 1).Should().BeTrue(
            "CLI should exit 0 (graceful handling) or 1 (error), not crash");
    }

    [Fact]
    public async Task Url_AcceptsFormatOption()
    {
        var cli = new CliRunner();
        // Verify that --format json is accepted as a valid option (even if the URL fails)
        await cli.RunAsync("url https://invalid.test.example --format json -q");

        // We expect an error (network failure) but NOT a parse error
        // stderr should contain the error, not a usage message about unknown options
        cli.Stderr.Should().NotContain("Unrecognized command or argument");
    }
}
