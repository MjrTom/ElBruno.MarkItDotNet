using System.Text;
using ClosedXML.Excel;

namespace ElBruno.MarkItDotNet.Excel;

/// <summary>
/// Converts Excel (.xlsx, .xlsm) files to Markdown tables using ClosedXML.
/// </summary>
public class ExcelConverter : IMarkdownConverter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".xlsx",
        ".xlsm"
    };

    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        SupportedExtensions.Contains(fileExtension);

    /// <inheritdoc />
    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var workbook = new XLWorkbook(fileStream);
        var sb = new StringBuilder();

        foreach (var worksheet in workbook.Worksheets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            sb.AppendLine($"## Sheet: {worksheet.Name}");
            sb.AppendLine();

            var rangeUsed = worksheet.RangeUsed();
            if (rangeUsed is null)
            {
                continue;
            }

            var rows = rangeUsed.RowsUsed().ToList();
            if (rows.Count == 0)
            {
                continue;
            }

            var columnCount = rangeUsed.ColumnCount();

            // Header row
            var headerRow = rows[0];
            sb.Append('|');
            for (var col = 1; col <= columnCount; col++)
            {
                var cellValue = headerRow.Cell(col).GetFormattedString() ?? string.Empty;
                sb.Append($" {EscapePipe(cellValue)} |");
            }
            sb.AppendLine();

            // Separator row
            sb.Append('|');
            for (var col = 1; col <= columnCount; col++)
            {
                sb.Append(" --- |");
            }
            sb.AppendLine();

            // Data rows
            for (var i = 1; i < rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = rows[i];
                sb.Append('|');
                for (var col = 1; col <= columnCount; col++)
                {
                    var cellValue = row.Cell(col).GetFormattedString() ?? string.Empty;
                    sb.Append($" {EscapePipe(cellValue)} |");
                }
                sb.AppendLine();
            }

            sb.AppendLine();
        }

        return Task.FromResult(sb.ToString().TrimEnd() + Environment.NewLine);
    }

    private static string EscapePipe(string value) =>
        value.Replace("|", "\\|");
}
