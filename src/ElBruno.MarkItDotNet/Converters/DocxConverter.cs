using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ElBruno.MarkItDotNet.Converters;

/// <summary>
/// Converts Word documents (.docx) to Markdown using DocumentFormat.OpenXml.
/// Extracts headings, paragraphs, bold/italic formatting, lists, and tables.
/// </summary>
public class DocxConverter : IMarkdownConverter
{
    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Task<string> ConvertAsync(Stream fileStream, string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var doc = WordprocessingDocument.Open(fileStream, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body is null)
        {
            return Task.FromResult(string.Empty);
        }

        var sb = new StringBuilder();

        foreach (var element in body.Elements())
        {
            switch (element)
            {
                case Paragraph paragraph:
                    ProcessParagraph(paragraph, sb);
                    break;
                case Table table:
                    ProcessTable(table, sb);
                    break;
            }
        }

        return Task.FromResult(sb.ToString().TrimEnd());
    }

    private static void ProcessParagraph(Paragraph paragraph, StringBuilder sb)
    {
        var styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        var headingLevel = GetHeadingLevel(styleId);

        var text = GetFormattedText(paragraph);
        if (string.IsNullOrWhiteSpace(text) && headingLevel == 0)
        {
            sb.AppendLine();
            return;
        }

        if (headingLevel > 0)
        {
            sb.Append(new string('#', headingLevel));
            sb.Append(' ');
        }

        // Check for list items
        var numProps = paragraph.ParagraphProperties?.NumberingProperties;
        if (numProps is not null)
        {
            var level = numProps.NumberingLevelReference?.Val?.Value ?? 0;
            var indent = new string(' ', level * 2);
            sb.Append(indent);
            sb.Append("- ");
        }

        sb.AppendLine(text);
        sb.AppendLine();
    }

    private static string GetFormattedText(Paragraph paragraph)
    {
        var sb = new StringBuilder();

        foreach (var run in paragraph.Elements<Run>())
        {
            var runText = string.Concat(run.Elements<Text>().Select(t => t.Text));
            if (string.IsNullOrEmpty(runText))
                continue;

            var props = run.RunProperties;
            var isBold = props?.Bold is not null || props?.Bold?.Val?.Value == true;
            var isItalic = props?.Italic is not null || props?.Italic?.Val?.Value == true;

            if (isBold && isItalic)
                sb.Append($"***{runText}***");
            else if (isBold)
                sb.Append($"**{runText}**");
            else if (isItalic)
                sb.Append($"*{runText}*");
            else
                sb.Append(runText);
        }

        return sb.ToString();
    }

    private static int GetHeadingLevel(string? styleId)
    {
        if (string.IsNullOrEmpty(styleId))
            return 0;

        // Word heading styles: Heading1, Heading2, ... or heading 1, heading 2, ...
        if (styleId.StartsWith("Heading", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(styleId.AsSpan(7), out var level) &&
            level is >= 1 and <= 6)
        {
            return level;
        }

        return 0;
    }

    private static void ProcessTable(Table table, StringBuilder sb)
    {
        var rows = table.Elements<TableRow>().ToList();
        if (rows.Count == 0)
            return;

        // First row as header
        var headerCells = rows[0].Elements<TableCell>().ToList();
        sb.Append('|');
        foreach (var cell in headerCells)
        {
            var text = GetCellText(cell);
            sb.Append($" {text} |");
        }
        sb.AppendLine();

        // Separator
        sb.Append('|');
        foreach (var _ in headerCells)
        {
            sb.Append(" --- |");
        }
        sb.AppendLine();

        // Data rows
        for (var i = 1; i < rows.Count; i++)
        {
            var cells = rows[i].Elements<TableCell>().ToList();
            sb.Append('|');
            foreach (var cell in cells)
            {
                var text = GetCellText(cell);
                sb.Append($" {text} |");
            }
            sb.AppendLine();
        }

        sb.AppendLine();
    }

    private static string GetCellText(TableCell cell)
    {
        return string.Join(" ", cell.Elements<Paragraph>()
            .Select(p => string.Concat(p.Elements<Run>()
                .SelectMany(r => r.Elements<Text>())
                .Select(t => t.Text))))
            .Trim();
    }
}
