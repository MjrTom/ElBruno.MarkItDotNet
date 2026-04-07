using System.CommandLine;
using System.CommandLine.Invocation;
using ElBruno.MarkItDotNet.Cli.Commands;

// === Root command: markitdown <file> ===
var fileArg = new Argument<FileInfo>(
    name: "file",
    description: "File to convert to Markdown");

var outputOption = new Option<FileInfo?>(
    ["-o", "--output"],
    "Write output to file instead of stdout");

var formatOption = new Option<string>(
    ["--format"],
    () => "markdown",
    "Output format: markdown or json");

var streamingOption = new Option<bool>(
    ["--streaming"],
    "Use streaming API for supported formats");

var quietOption = new Option<bool>(
    ["-q", "--quiet"],
    "Suppress status messages on stderr");

var verboseOption = new Option<bool>(
    ["-v", "--verbose"],
    "Show metadata (word count, timing, format)");

var rootCommand = new RootCommand("Convert files to Markdown — powered by ElBruno.MarkItDotNet")
{
    fileArg,
    outputOption,
    formatOption,
    streamingOption,
    quietOption,
    verboseOption
};

rootCommand.SetHandler(async (InvocationContext ctx) =>
{
    var ct = ctx.GetCancellationToken();
    ctx.ExitCode = await ConvertCommand.HandleAsync(
        ctx.ParseResult.GetValueForArgument(fileArg),
        ctx.ParseResult.GetValueForOption(outputOption),
        ctx.ParseResult.GetValueForOption(formatOption)!,
        ctx.ParseResult.GetValueForOption(streamingOption),
        ctx.ParseResult.GetValueForOption(quietOption),
        ctx.ParseResult.GetValueForOption(verboseOption),
        ct);
});

// === batch subcommand ===
var batchDirArg = new Argument<DirectoryInfo>(
    name: "directory",
    description: "Directory containing files to convert");

var batchOutputOption = new Option<DirectoryInfo>(
    ["-o", "--output"],
    "Output directory for converted files")
{ IsRequired = true };

var recursiveOption = new Option<bool>(
    ["-r", "--recursive"],
    "Process subdirectories recursively");

var patternOption = new Option<string?>(
    ["--pattern"],
    "File glob pattern (e.g. *.pdf)");

var parallelOption = new Option<int>(
    ["--parallel"],
    () => Environment.ProcessorCount,
    "Max parallel conversions");

var batchFormatOption = new Option<string>(
    ["--format"],
    () => "markdown",
    "Output format: markdown or json");

var batchQuietOption = new Option<bool>(
    ["-q", "--quiet"],
    "Suppress status messages on stderr");

var batchCommand = new Command("batch", "Batch-convert all files in a directory")
{
    batchDirArg,
    batchOutputOption,
    recursiveOption,
    patternOption,
    parallelOption,
    batchFormatOption,
    batchQuietOption
};

batchCommand.SetHandler(async (InvocationContext ctx) =>
{
    var ct = ctx.GetCancellationToken();
    ctx.ExitCode = await BatchCommand.HandleAsync(
        ctx.ParseResult.GetValueForArgument(batchDirArg),
        ctx.ParseResult.GetValueForOption(batchOutputOption)!,
        ctx.ParseResult.GetValueForOption(recursiveOption),
        ctx.ParseResult.GetValueForOption(patternOption),
        ctx.ParseResult.GetValueForOption(parallelOption),
        ctx.ParseResult.GetValueForOption(batchFormatOption)!,
        ctx.ParseResult.GetValueForOption(batchQuietOption),
        ct);
});

rootCommand.AddCommand(batchCommand);

// === url subcommand ===
var urlArg = new Argument<string>(
    name: "url",
    description: "HTTP/HTTPS URL to fetch and convert");

var urlOutputOption = new Option<FileInfo?>(
    ["-o", "--output"],
    "Write output to file instead of stdout");

var urlFormatOption = new Option<string>(
    ["--format"],
    () => "markdown",
    "Output format: markdown or json");

var urlQuietOption = new Option<bool>(
    ["-q", "--quiet"],
    "Suppress status messages on stderr");

var urlCommand = new Command("url", "Convert a web page URL to Markdown")
{
    urlArg,
    urlOutputOption,
    urlFormatOption,
    urlQuietOption
};

urlCommand.SetHandler(async (InvocationContext ctx) =>
{
    var ct = ctx.GetCancellationToken();
    ctx.ExitCode = await UrlCommand.HandleAsync(
        ctx.ParseResult.GetValueForArgument(urlArg),
        ctx.ParseResult.GetValueForOption(urlOutputOption),
        ctx.ParseResult.GetValueForOption(urlFormatOption)!,
        ctx.ParseResult.GetValueForOption(urlQuietOption),
        ct);
});

rootCommand.AddCommand(urlCommand);

// === formats subcommand ===
var formatsCommand = new Command("formats", "List all supported file formats and their converters");

formatsCommand.SetHandler(async (InvocationContext ctx) =>
{
    ctx.ExitCode = await FormatsCommand.HandleAsync();
});

rootCommand.AddCommand(formatsCommand);

// Run
return await rootCommand.InvokeAsync(args);
