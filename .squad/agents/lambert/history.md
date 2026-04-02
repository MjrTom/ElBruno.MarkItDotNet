# Lambert — History

## Project Context
- **Project:** ElBruno.MarkItDotNet — .NET library converting file formats to Markdown
- **User:** Bruno Capuano
- **Stack:** .NET 8/10, C#, xUnit, NuGet, GitHub Actions
- **Reference repo:** ElBruno.LocalLLMs — patterns to replicate:
  - Directory.Build.props with shared package metadata
  - .slnx XML-based solution format
  - global.json with sdk 8.0.0, rollForward latestMajor
  - CI workflow: build + test on PR (net8.0 only)
  - Publish workflow: triggered on release, OIDC NuGet auth, version from tag
  - nuget_logo.png in /images/ directory
  - README.md packed into NuGet package

## Learnings
