# scheduling-lib — Development Guidelines

## Platform & Language

- Always target the **latest stable .NET** (currently .NET 9) and the **latest stable C#** (currently C# 13).
- Use `<LangVersion>latest</LangVersion>` in `Directory.Build.props` so it tracks automatically.
- When a new stable .NET release lands, update `<TargetFramework>` in `Directory.Build.props` and all package versions.

---

## Solution Structure

On disk, source and test projects are separated into `src/` and `tests/`. In the Solution View (Visual Studio / Rider), both appear at the same top level.

```
scheduling-lib/
├── src/
│   └── ProjectName/
│       └── ProjectName.csproj
├── tests/
│   └── ProjectName.Tests/
│       └── ProjectName.Tests.csproj
├── Directory.Build.props
├── Directory.Packages.props
└── SchedulingLib.sln
```

To add both projects to the solution at the same level:

```bash
dotnet sln add src/ProjectName/ProjectName.csproj
dotnet sln add tests/ProjectName.Tests/ProjectName.Tests.csproj
```

---

## Central Package Management (CPM)

All NuGet package versions are declared once in `Directory.Packages.props` at the repo root.

**`Directory.Packages.props`** (template):

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Test packages -->
    <PackageVersion Include="xunit" Version="2.9.3" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageVersion Include="Moq" Version="4.20.72" />
    <PackageVersion Include="Testcontainers" Version="3.10.0" />
    <!-- Production packages go here -->
  </ItemGroup>
</Project>
```

Individual `.csproj` files reference packages **without** a `Version` attribute:

```xml
<PackageReference Include="Moq" />
```

When adding a new package: add it to `Directory.Packages.props` first, then reference it in the project.

---

## Directory.Build.props

Shared build configuration lives in `Directory.Build.props` at the repo root. Individual projects should contain only what is unique to them.

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
```

---

## Model Design: Records vs Classes

| Scenario | Type to use |
|---|---|
| Model is mostly read after creation; few or no mutations | `record` |
| Model undergoes many changes over its lifetime | `class` |

Use `record` for DTOs, value objects, and result types. Use `class` for entities or objects with rich mutable state.

---

## Testing

- Every source project `{ProjectName}` has a paired test project `{ProjectName}.Tests` in `tests/`.
- **Default to unit tests.** Do not write integration tests unless explicitly asked.
- Test framework: **xUnit**
- Mocking: **Moq**
- Integration tests (only when requested): **Testcontainers** to spin up real DB containers.

Minimal test `.csproj` template:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="Moq" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\ProjectName\ProjectName.csproj" />
  </ItemGroup>
</Project>
```

---

## XML Documentation

All public classes and methods must have `<summary>` XML doc comments. Keep them in sync with the implementation — a stale summary is worse than none.

```csharp
/// <summary>
/// Calculates the next available scheduling slot after the given start time.
/// </summary>
public DateTimeOffset GetNextSlot(DateTimeOffset start) { ... }
```

`<GenerateDocumentationFile>true</GenerateDocumentationFile>` is already set in `Directory.Build.props`, so missing docs will surface as warnings (treated as errors).
