# Squad Decisions

## Active Decisions

### Phase 1 Scaffolding Patterns

**Author:** Lambert  
**Date:** 2025-07-18  
**Status:** Implemented  

Project scaffolding follows ElBruno.LocalLLMs patterns exactly:
- `.slnx` XML solution format (not `.sln`)
- `Directory.Build.props` for shared MSBuild/NuGet metadata
- `global.json` SDK 8.0.0 with `rollForward: latestMajor`
- Multi-target `net8.0;net10.0` for both library and test projects
- CI builds use `-p:TargetFrameworks=net8.0` to avoid SDK requirements
- NuGet pack includes README.md and nuget_logo.png from repo root

**Test Stack:** xUnit 2.9.0 + FluentAssertions 8.3.0, coverlet.collector for coverage

**Notes:** Icon at `images/nuget_logo.png` is a placeholder. `/src/samples/` folder exists in .slnx but is empty, ready for future samples.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
