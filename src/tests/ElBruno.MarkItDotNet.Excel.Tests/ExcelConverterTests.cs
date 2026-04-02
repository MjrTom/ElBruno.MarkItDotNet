using ClosedXML.Excel;
using FluentAssertions;
using Xunit;

namespace ElBruno.MarkItDotNet.Excel.Tests;

public class ExcelConverterTests
{
    private readonly ExcelConverter _converter = new();

    [Fact]
    public void CanHandle_Xlsx_ReturnsTrue()
    {
        _converter.CanHandle(".xlsx").Should().BeTrue();
    }

    [Fact]
    public void CanHandle_Xlsm_ReturnsTrue()
    {
        _converter.CanHandle(".xlsm").Should().BeTrue();
    }

    [Theory]
    [InlineData(".docx")]
    [InlineData(".pdf")]
    [InlineData(".txt")]
    [InlineData(".csv")]
    [InlineData(".pptx")]
    public void CanHandle_Other_ReturnsFalse(string extension)
    {
        _converter.CanHandle(extension).Should().BeFalse();
    }

    [Fact]
    public async Task ConvertAsync_SimpleSpreadsheet_ReturnsMarkdownTable()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws = wb.Worksheets.Add("Data");
            ws.Cell("A1").Value = "Name";
            ws.Cell("B1").Value = "Age";
            ws.Cell("A2").Value = "Alice";
            ws.Cell("B2").Value = 30;
            ws.Cell("A3").Value = "Bob";
            ws.Cell("B3").Value = 25;
        });

        var result = await _converter.ConvertAsync(stream, ".xlsx");

        result.Should().Contain("## Sheet: Data");
        result.Should().Contain("| Name | Age |");
        result.Should().Contain("| --- | --- |");
        result.Should().Contain("| Alice | 30 |");
        result.Should().Contain("| Bob | 25 |");
    }

    [Fact]
    public async Task ConvertAsync_MultipleSheets_ReturnsAllSheets()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws1 = wb.Worksheets.Add("Sheet1");
            ws1.Cell("A1").Value = "Header1";
            ws1.Cell("A2").Value = "Value1";

            var ws2 = wb.Worksheets.Add("Sheet2");
            ws2.Cell("A1").Value = "Header2";
            ws2.Cell("A2").Value = "Value2";
        });

        var result = await _converter.ConvertAsync(stream, ".xlsx");

        result.Should().Contain("## Sheet: Sheet1");
        result.Should().Contain("## Sheet: Sheet2");
        result.Should().Contain("| Header1 |");
        result.Should().Contain("| Header2 |");
    }

    [Fact]
    public async Task ConvertAsync_EmptySheet_ReturnsEmptyOrMinimal()
    {
        using var stream = CreateExcelStream(wb =>
        {
            wb.Worksheets.Add("Empty");
        });

        var result = await _converter.ConvertAsync(stream, ".xlsx");

        result.Should().Contain("## Sheet: Empty");
        // No table content for an empty sheet
        result.Should().NotContain("| --- |");
    }

    [Fact]
    public void Plugin_RegistersConverter()
    {
        var plugin = new ExcelPlugin();

        plugin.Name.Should().Be("Excel");
        plugin.GetConverters().Should().ContainSingle()
            .Which.Should().BeOfType<ExcelConverter>();
    }

    private static MemoryStream CreateExcelStream(Action<XLWorkbook> configure)
    {
        var ms = new MemoryStream();
        using (var wb = new XLWorkbook())
        {
            configure(wb);
            wb.SaveAs(ms);
        }
        ms.Position = 0;
        return ms;
    }
}
