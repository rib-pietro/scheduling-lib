# API Project — Development Guidelines

## Library Reference

This project consumes **scheduling-lib**. Before writing any domain logic, read the library README to understand the available packages, entities, services, and how to wire everything up:

**https://github.com/rib-pietro/scheduling-lib/blob/master/README.md**

Key things to know from that README before starting:

- The **Users domain** (`SchedulingLib.Users`) manages platform users (clients/guests) — `User`, `IUserService`, `RegisterUserRequest`. A user requires a name and at least one of email or phone. The user's `Id` is used as `client_id` in appointments and `guest_id` in reservations.
- The **Services domain** (`SchedulingLib.Services`) handles staff-based appointment booking — `ServiceType`, `StaffMember`, `ServiceAppointment`, `IServiceAppointmentService`, `IServiceTypeService`.
- The **Reservations domain** (`SchedulingLib.Reservations`) handles resource-based multi-night bookings — `ReservableResource`, `Reservation`, `IReservationService`.
- **PostgreSQL persistence** (`SchedulingLib.Persistence.PostgreSQL`) provides ready-made repository implementations for both Users and Services; call `PostgreSqlSchemaInitializer.InitializeAsync` at startup.
- All service and entity operations return `Result<T>` — always check `IsSuccess` before using `Value`.
- DI entry points are `AddUserScheduling().AddPostgreSqlPersistence(connectionString)`, `AddServiceScheduling().AddPostgreSqlPersistence(connectionString)`, and `AddReservationScheduling()`.

---

## Platform & Language

- Always target the **latest stable .NET** (currently .NET 10) and the **latest stable C#** (currently C# 13).
- Use `<LangVersion>latest</LangVersion>` in `Directory.Build.props` so it tracks automatically.
- When a new stable .NET release lands, update `<TargetFramework>` in `Directory.Build.props` and all package versions.

---

## Solution Structure

Source and test projects are separated into `src/` and `tests/` on disk; both appear at the top level in the Solution View.

```
my-api/
├── src/
│   └── ProjectName/
│       └── ProjectName.csproj
├── tests/
│   └── ProjectName.Tests/
│       └── ProjectName.Tests.csproj
├── Directory.Build.props
├── Directory.Packages.props
└── MyApi.sln
```

To add projects to the solution:

```bash
dotnet sln add src/ProjectName/ProjectName.csproj
dotnet sln add tests/ProjectName.Tests/ProjectName.Tests.csproj
```

---

## Central Package Management (CPM)

All NuGet package versions are declared once in `Directory.Packages.props` at the repo root.

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- scheduling-lib packages -->
    <PackageVersion Include="SchedulingLib.Users" Version="0.0.1" />
    <PackageVersion Include="SchedulingLib.Users.Extensions" Version="0.0.1" />
    <PackageVersion Include="SchedulingLib.Services" Version="0.0.1" />
    <PackageVersion Include="SchedulingLib.Services.Extensions" Version="0.0.1" />
    <PackageVersion Include="SchedulingLib.Persistence.PostgreSQL" Version="0.0.1" />
    <!-- add other packages here -->
  </ItemGroup>
</Project>
```

Individual `.csproj` files reference packages **without** a `Version` attribute:

```xml
<PackageReference Include="SchedulingLib.Services" />
```

When adding a new package: add it to `Directory.Packages.props` first, then reference it in the project.

---

## Directory.Build.props

Shared build configuration lives in `Directory.Build.props` at the repo root.

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

Individual projects should contain only what is unique to them.

---

## Model Design: Records vs Classes

| Scenario | Type to use |
|---|---|
| Model is mostly read after creation; few or no mutations | `record` |
| Model undergoes many changes over its lifetime | `class` |

Use `record` for DTOs, request/response models, and value objects. Use `class` for entities with rich mutable state.

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
/// Returns available booking slots for the given staff member on the given date.
/// </summary>
public Task<Result<IReadOnlyList<TimeSlot>>> GetAvailableSlotsAsync(Guid staffMemberId, DateOnly date) { ... }
```

`<GenerateDocumentationFile>true</GenerateDocumentationFile>` is set in `Directory.Build.props`, so missing docs surface as warnings treated as errors.
