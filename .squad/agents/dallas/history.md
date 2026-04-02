# Dallas — History

## Project Context
- **Project:** ElBruno.MarkItDotNet — .NET library converting file formats to Markdown
- **User:** Bruno Capuano
- **Stack:** .NET 8/10, C#, xUnit, NuGet, GitHub Actions
- **Architecture:** IMarkdownConverter interface with CanHandle(extension) + ConvertAsync(stream), ConverterRegistry resolves converters, MarkdownService is the main entry point
- **Formats (v1):** txt, html, pdf, docx, json, images (OCR optional)

## Learnings
