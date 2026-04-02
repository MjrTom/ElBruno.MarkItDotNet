using ClosedXML.Excel;
using ElBruno.MarkItDotNet;
using ElBruno.MarkItDotNet.Excel;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ElBruno.MarkItDotNet - Excel Conversion Sample          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();
services.AddMarkItDotNet();
services.AddMarkItDotNetExcel();
var serviceProvider = services.BuildServiceProvider();
var markdownService = serviceProvider.GetRequiredService<MarkdownService>();

// ── Build an Excel workbook in memory ────────────────────────────────────
Console.WriteLine("📊 Creating Excel workbook in memory...\n");

using var workbook = new XLWorkbook();

var sales = workbook.Worksheets.Add("Sales Data");
sales.Cell("A1").Value = "Product";
sales.Cell("B1").Value = "Q1";
sales.Cell("C1").Value = "Q2";
sales.Cell("D1").Value = "Q3";
string[][] rows = [
    ["Widget A", "1200", "1450", "1325"],
    ["Widget B", "800",  "920",  "1100"],
    ["Widget C", "3500", "3200", "3650"],
    ["Gadget X", "450",  "510",  "480"]
];
for (var i = 0; i < rows.Length; i++)
    for (var j = 0; j < rows[i].Length; j++)
        sales.Cell(i + 2, j + 1).Value = rows[i][j];

var summary = workbook.Worksheets.Add("Summary");
summary.Cell("A1").Value = "Metric";
summary.Cell("B1").Value = "Value";
summary.Cell("A2").Value = "Total Products";
summary.Cell("B2").Value = "4";
summary.Cell("A3").Value = "Best Quarter";
summary.Cell("B3").Value = "Q3";
summary.Cell("A4").Value = "Top Product";
summary.Cell("B4").Value = "Widget C";

// ── Convert to Markdown ──────────────────────────────────────────────────
Console.WriteLine("📝 Converting to Markdown...");
Console.WriteLine("─────────────────────────────────────────────────────────\n");

using var ms = new MemoryStream();
workbook.SaveAs(ms);
ms.Position = 0;

var result = await markdownService.ConvertAsync(ms, ".xlsx");
if (result.Success)
{
    Console.WriteLine("✅ Excel conversion succeeded!");
    Console.WriteLine($"   Source format: {result.SourceFormat}\n");
    Console.WriteLine("Converted Markdown:");
    Console.WriteLine(result.Markdown);
}
else
{
    Console.WriteLine($"❌ Conversion failed: {result.ErrorMessage}");
}

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║             Excel Sample Complete!                       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
