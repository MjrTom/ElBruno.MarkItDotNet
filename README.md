# ElBruno.MarkItDotNet

[![NuGet](https://img.shields.io/nuget/v/ElBruno.MarkItDotNet.svg)](https://www.nuget.org/packages/ElBruno.MarkItDotNet)
[![Build](https://github.com/elbruno/ElBruno.MarkItDotNet/actions/workflows/ci.yml/badge.svg)](https://github.com/elbruno/ElBruno.MarkItDotNet/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**.NET library that converts file formats to Markdown** for AI pipelines, documentation workflows, and developer tools. Inspired by Python [markitdown](https://github.com/microsoft/markitdown).

## Installation

```bash
dotnet add package ElBruno.MarkItDotNet
```

## Quick Start

```csharp
using ElBruno.MarkItDotNet;

var converter = new MarkdownConverter();
var markdown = converter.ConvertToMarkdown("document.txt");
Console.WriteLine(markdown);
```

## Supported Formats

| Format | Extension | Status |
|--------|-----------|--------|
| Plain Text | `.txt` | ✅ Supported |
| HTML | `.html` | 🔜 Planned |
| PDF | `.pdf` | 🔜 Planned |
| Word | `.docx` | 🔜 Planned |
| Excel | `.xlsx` | 🔜 Planned |
| PowerPoint | `.pptx` | 🔜 Planned |
| CSV | `.csv` | 🔜 Planned |
| JSON | `.json` | 🔜 Planned |
| XML | `.xml` | 🔜 Planned |

## Target Frameworks

- .NET 8.0 (LTS)
- .NET 10.0

## Building

```bash
dotnet build ElBruno.MarkItDotNet.slnx
```

## Testing

```bash
dotnet test ElBruno.MarkItDotNet.slnx
```

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## Author

**Bruno Capuano** ([@elbruno](https://github.com/elbruno))
