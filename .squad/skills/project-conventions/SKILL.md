---
name: "project-conventions"
description: "Core conventions and patterns for ElBruno.MarkItDotNet"
domain: "project-conventions"
confidence: "high"
source: "lambert-phase1"
---

## Context

ElBruno.MarkItDotNet follows the same project patterns as ElBruno.LocalLLMs.

## Patterns

### Solution Format
- Use `.slnx` (XML-based), never `.sln`
- Folder structure: `/src/`, `/src/tests/`, `/src/samples/`

### Target Frameworks
- Libraries: `net8.0;net10.0` (multi-target)
- Tests: `net8.0;net10.0` (multi-target)
- CI builds: `-p:TargetFrameworks=net8.0` to avoid SDK requirement

### NuGet Packaging
- Shared metadata in `Directory.Build.props` (authors, license, icon)
- Per-project: PackageId, Description, Tags, Version in .csproj
- Pack README.md and images/nuget_logo.png via ItemGroup with relative paths

### Testing
- Framework: xUnit 2.9.0
- Assertions: FluentAssertions 8.3.0
- Coverage: coverlet.collector
- Location: `src/tests/{ProjectName}.Tests/`
- Run: `dotnet test ElBruno.MarkItDotNet.slnx`

### Code Style
- `.editorconfig` at repo root
- File-scoped namespaces (warning level)
- `var` preferred everywhere
- Private fields: `_camelCase`

### File Structure
```
├── Directory.Build.props, global.json, .editorconfig
├── ElBruno.MarkItDotNet.slnx
├── src/ElBruno.MarkItDotNet/          (core library)
├── src/tests/ElBruno.MarkItDotNet.Tests/
├── images/nuget_logo.png
├── README.md, LICENSE
```

## Anti-Patterns
- **Never use .sln** — always .slnx XML format
- **Never hardcode version year** — use `$([System.DateTime]::Now.Year)` in Directory.Build.props
