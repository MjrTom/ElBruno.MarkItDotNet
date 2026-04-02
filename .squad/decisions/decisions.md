# Decisions — ElBruno.MarkItDotNet

---

## Decision: Converter NuGet Package Choices

**Author:** Dallas (Backend Developer)  
**Date:** 2025-07-18  
**Status:** Implemented  

### Context
Phases 3-6 required adding HTML, DOCX, PDF, and Image converters to the library. Each needed a NuGet dependency for parsing the source format.

### Decisions

#### ReverseMarkdown 4.6.0 — HTML to Markdown
- **Why:** Purpose-built for HTML→Markdown conversion. Handles headings, lists, links, images, tables, bold/italic, and code blocks out of the box.
- **Config:** UnknownTags=PassThrough, RemoveComments=true, GithubFlavored=true for GitHub-compatible output.
- **Alternatives considered:** Manual regex/HtmlAgilityPack — too error-prone for complex HTML.

#### DocumentFormat.OpenXml 3.3.0 — DOCX parsing
- **Why:** Microsoft's official SDK for Open XML formats. Zero native dependencies, pure .NET. Supports reading Word styles (headings), run properties (bold/italic), tables, and numbering (lists).
- **Alternatives considered:** NPOI — heavier dependency, less direct access to style info.

#### PdfPig 0.1.14 — PDF text extraction
- **Why:** Pure .NET PDF reader, no native dependencies. Extracts text page-by-page. Lightweight and well-suited for text extraction use case.
- **Note:** The NuGet package ID is `PdfPig`, NOT `UglyToad.PdfPig` (which only has pre-release builds).
- **Alternatives considered:** iTextSharp (AGPL licensed), PdfSharp (limited text extraction).

#### ImageConverter — No external dependency
- **Why:** v1 only generates markdown image references (`![Image](filename)`) and reads image dimensions from file headers (PNG, JPEG, GIF, BMP) using raw byte parsing. No heavy imaging libraries needed.
- **OCR:** Stubbed behind `MarkItDotNetOptions.EnableOcr` for v2 implementation.

### Impact
- Three new NuGet dependencies added to ElBruno.MarkItDotNet.csproj
- All converters follow the IMarkdownConverter pattern (CanHandle + ConvertAsync)
- Registered in both DI (ServiceCollectionExtensions) and sync façade (MarkdownConverter)

---

## Decision: Test Patterns for ElBruno.MarkItDotNet

**Author:** Parker  
**Date:** 2025-07-18  
**Status:** Implemented

### Converter Test Structure

Each converter test class follows a consistent pattern:

1. **CanHandle** — verify supported extensions (including case-insensitive), and verify rejection of unsupported extensions via `[Theory]`/`[InlineData]`
2. **NullStream** — verify `ArgumentNullException` for null stream input
3. **Empty/Whitespace input** — verify graceful handling (empty string or no crash)
4. **Happy path** — convert known input, assert key markers in output (don't assert exact strings — converters may evolve formatting)
5. **TestData golden files** — when practical, read from `TestData/` directory and compare against `.expected.md` files

### Key Patterns

- **In-memory file creation** for binary formats (DOCX via OpenXml, PNG/GIF/BMP via raw headers, PDF via raw syntax) — no test fixture files on disk needed for most tests
- **Contract-first testing** — test against `IMarkdownConverter` interface behavior, not implementation details
- **StubConverter** inner class in ConverterRegistryTests for testing registry behavior without depending on real converters
- **Loose assertions** — use `Should().Contain()` over `Should().Be()` for converter output to tolerate formatting changes

### TestData Directory

- Located at `src/tests/ElBruno.MarkItDotNet.Tests/TestData/`
- Requires `<None Update="TestData\**\*" CopyToOutputDirectory="PreserveNewest" />` in test csproj
- Tests gracefully skip if TestData files aren't found (for CI resilience)
