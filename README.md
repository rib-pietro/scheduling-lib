# scheduling-lib

Domain logic, entities, and PostgreSQL persistence for two scheduling patterns:

- **Services** — staff-based appointment booking (e.g. hairdressers, therapists, consultants)
- **Reservations** — resource-based multi-night bookings (e.g. rental properties, accommodation)

---

## Packages

All packages target **net10.0** and are published to [GitHub Packages](https://github.com/rib-pietro/scheduling-lib/packages).

| Package | Purpose |
|---|---|
| `SchedulingLib.Core` | Primitives, `Result<T>`, calendar connector abstraction |
| `SchedulingLib.Users` | Users domain — `User` entity, `IUserRepository`, `IUserService` |
| `SchedulingLib.Users.Extensions` | DI registration via `AddUserScheduling()` |
| `SchedulingLib.Services` | Services domain — entities, value objects, repository and service interfaces |
| `SchedulingLib.Services.Extensions` | DI registration via `AddServiceScheduling()` |
| `SchedulingLib.Persistence.PostgreSQL` | PostgreSQL implementation of all Users and Services repositories |
| `SchedulingLib.Reservations` | Reservations domain — entities, value objects, repository and service interface |
| `SchedulingLib.Reservations.Extensions` | DI registration via `AddReservationScheduling()` |

### Adding the NuGet source

GitHub Packages requires authentication even for public packages. Create a Personal Access Token with `read:packages` scope at **github.com → Settings → Developer settings → Personal access tokens**, then:

```bash
dotnet nuget add source \
  --username <your-github-username> \
  --password <PAT> \
  --store-password-in-clear-text \
  --name github \
  "https://nuget.pkg.github.com/rib-pietro/index.json"
```

Then reference packages without a version (use Central Package Management, or specify `Version="1.0.0"`):

```bash
dotnet add package SchedulingLib.Services
dotnet add package SchedulingLib.Services.Extensions
dotnet add package SchedulingLib.Persistence.PostgreSQL
```

---

## Users Domain

Register platform users who act as clients in appointments or guests in reservations. A user requires a name and at least one contact method (email or phone).

### Quick Start

```csharp
builder.Services
    .AddUserScheduling()
    .AddPostgreSqlPersistence(connectionString);

await PostgreSqlSchemaInitializer.InitializeAsync(connectionString);
```

### User

```csharp
// Register via the service (validates name + email-or-phone rule)
var request = new RegisterUserRequest(
    Name: "Alice",
    Email: "alice@example.com",
    Phone: null);                    // at least one of Email or Phone is required

Result<User> result = await userService.RegisterAsync(request);

if (result.IsSuccess)
{
    User user = result.Value!;
    // user.Id is the Guid used as client_id / guest_id in appointments and reservations
}
```

### IUserService

```csharp
Task<Result<User>> RegisterAsync(RegisterUserRequest request, ...)
Task<User?>         GetByIdAsync(Guid id, ...)
```

### IUserRepository

```csharp
Task<User?> GetByIdAsync(Guid id, ...);
Task        SaveAsync(User user, ...);
```

---

## Services Domain

Use this domain when a **person** (staff member) delivers a **service** to a **client** in a bookable time slot.

### Quick Start

```csharp
// Program.cs
builder.Services
    .AddServiceScheduling()
    .AddPostgreSqlPersistence(connectionString);

// Once at startup — idempotent, safe to call on every deploy
await PostgreSqlSchemaInitializer.InitializeAsync(connectionString);
```

### Concepts

#### ServiceType

Describes a service that staff members can offer. Identified by ID and carries a name, price, and duration.

```csharp
record ServiceType(Guid Id, string Name, decimal Price, TimeSpan Duration);
```

Managed via `IServiceTypeService`:

```csharp
// Create
Result<ServiceType> result = await serviceTypeService.CreateAsync("Haircut", 25.00m, TimeSpan.FromMinutes(60));

// List all
Result<IReadOnlyList<ServiceType>> all = await serviceTypeService.GetAllAsync();

// Delete
Result<bool> deleted = await serviceTypeService.DeleteAsync(id);
```

#### StaffMember

A person who delivers services. Holds a weekly availability schedule, a list of offered services, an optional profile picture, and a photo gallery.

```csharp
var schedule = new WeeklySchedule([
    new DayOfWeekSchedule(DayOfWeek.Monday,    [new TimeSlot(new TimeOnly(9,0), new TimeOnly(17,0))]),
    new DayOfWeekSchedule(DayOfWeek.Wednesday, [new TimeSlot(new TimeOnly(9,0), new TimeOnly(13,0))]),
]);

var staff = new StaffMember(Guid.NewGuid(), "Alice", "alice@example.com", schedule);

// Manage offered services
staff.AddService(haircut);         // Result
staff.RemoveService(haircut.Id);   // Result

// Profile media
staff.UpdateProfilePicture("https://cdn.example.com/alice.jpg");
staff.AddGalleryPhoto(new GalleryPhoto("https://cdn.example.com/work1.jpg", [haircut.Id], DateTimeOffset.UtcNow));
staff.RemoveGalleryPhoto("https://cdn.example.com/work1.jpg");  // Result

// Schedule
staff.UpdateSchedule(newSchedule);
```

Persist via `IStaffMemberRepository`:

```csharp
await staffRepo.SaveAsync(staff);
StaffMember? found = await staffRepo.GetByIdAsync(id);
```

#### ServiceAppointment

A single-day, time-bounded appointment. Lifecycle: **Pending → Confirmed → Cancelled**.

```csharp
// Book via the service (validates schedule + conflicts)
var request = new BookAppointmentRequest(
    StaffMemberId: alice.Id,
    ClientId: clientId,
    ServiceType: haircut,
    Date: new DateOnly(2025, 9, 15),
    RequestedSlot: new TimeSlot(new TimeOnly(10, 0), new TimeOnly(11, 0)));

Result<ServiceAppointment> result = await appointmentService.BookAsync(request);

// Lifecycle transitions
appointment.Confirm();   // Result — must be Pending
appointment.Cancel();    // Result — must not be Cancelled

// Persist changes
await appointmentRepo.SaveAsync(appointment);
```

#### IServiceAppointmentService

The top-level orchestration service. Inject this in your API controllers/handlers.

```csharp
// Book — validates against the staff member's weekly schedule and existing bookings
Task<Result<ServiceAppointment>> BookAsync(BookAppointmentRequest request, ...)

// Cancel — transitions to Cancelled and optionally removes the calendar event
Task<Result<bool>> CancelAsync(Guid appointmentId, ...)

// Query free slots on a given date
Task<Result<IReadOnlyList<TimeSlot>>> GetAvailableSlotsAsync(Guid staffMemberId, DateOnly date, ...)
```

#### Repository interfaces

Implement these if you bring your own storage, or use the provided PostgreSQL implementation.

```csharp
public interface IStaffMemberRepository
{
    Task<StaffMember?> GetByIdAsync(Guid id, ...);
    Task SaveAsync(StaffMember staffMember, ...);
}

public interface IServiceAppointmentRepository
{
    Task<ServiceAppointment?> GetByIdAsync(Guid id, ...);
    Task SaveAsync(ServiceAppointment appointment, ...);
    Task<IReadOnlyList<ServiceAppointment>> GetByStaffMemberAndDateAsync(Guid staffMemberId, DateOnly date, ...);
}

public interface IServiceTypeRepository
{
    Task<ServiceType?> GetByIdAsync(Guid id, ...);
    Task<IReadOnlyList<ServiceType>> GetAllAsync(...);
    Task SaveAsync(ServiceType serviceType, ...);
    Task DeleteAsync(Guid id, ...);
}
```

---

## Reservations Domain

Use this domain when a **resource** (e.g. a rental unit) is booked by a **guest** for a date range.

### Quick Start

```csharp
builder.Services.AddReservationScheduling();
```

The reservations domain does not ship with a built-in persistence implementation — provide your own `IReservationRepository` and `IResourceRepository`.

### Concepts

#### ReservableResource

A named resource with a maximum occupancy and a weekly availability window that controls which check-in days are valid.

```csharp
record ReservableResource(
    Guid Id,
    string Name,
    string Description,
    int MaxOccupancy,
    WeeklySchedule AvailabilityWindow);
```

#### Reservation

A multi-night booking. Lifecycle: **PendingConfirmation → Confirmed → CheckedIn → CheckedOut**, with **Cancelled** possible from any non-final state.

```csharp
var reservation = new Reservation(
    id: Guid.NewGuid(),
    title: "Beach House",
    resourceId: beachHouse.Id,
    guestId: guestId,
    dateRange: new DateRange(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 7)),
    pricing: new ReservationPricingSnapshot(nightlyRate: 150m, nights: 6, total: 900m));

reservation.Confirm();   // Result — must be PendingConfirmation
reservation.CheckIn();   // Result — must be Confirmed
reservation.CheckOut();  // Result — must be CheckedIn
reservation.Cancel();    // Result — must not be Cancelled or CheckedOut
```

#### IReservationService

```csharp
// Create a reservation (validates availability window + overlaps)
Task<Result<Reservation>> ReserveAsync(CreateReservationRequest request, ...)

// Cancel
Task<Result<bool>> CancelAsync(Guid reservationId, ...)

// Availability checks
Task<Result<bool>> IsAvailableAsync(Guid resourceId, DateRange range, ...)
Task<Result<IReadOnlyList<DateRange>>> GetUnavailableDatesAsync(Guid resourceId, int year, int month, ...)
```

---

## Core Primitives

### TimeSlot

A contiguous window within a single day. `End` must be after `Start`.

```csharp
var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
slot.Duration   // TimeSpan
slot.Contains(other)  // bool — true when other fits entirely inside
```

### WeeklySchedule

Defines availability by day of week. Used as the guard for booking and reservation validation.

```csharp
var schedule = new WeeklySchedule([
    new DayOfWeekSchedule(DayOfWeek.Monday, [new TimeSlot(new TimeOnly(9,0), new TimeOnly(17,0))])
]);

schedule.GetSlotsFor(date);          // IReadOnlyList<TimeSlot>
schedule.Contains(date, slot);       // bool — slot falls within an available window
schedule.ContainsDay(date);          // bool — day of week is present in the schedule
```

### DateRange

An inclusive date range. `End` must not be before `Start`.

```csharp
var range = new DateRange(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 7));
range.Nights          // int
range.Overlaps(other) // bool
range.Contains(date)  // bool
```

### Result\<T\>

A railway-oriented return type used by all service and entity methods.

```csharp
Result ok     = Result.Ok();
Result fail   = Result.Fail("Something went wrong.");

Result<ServiceType> okValue  = Result.Ok(serviceType);
Result<ServiceType> failType = Result.Fail<ServiceType>("Not found.");

if (result.IsSuccess)
    use(result.Value);
else
    log(result.ErrorMessage);
```

---

## ICalendarConnector

An optional abstraction for syncing scheduling events with external calendar providers (e.g. Outlook, iCal). Implement this interface and register it with the builder to enable automatic calendar sync on booking/cancellation.

```csharp
public interface ICalendarConnector
{
    string ProviderName { get; }
    Task<Result<string>>                      CreateEventAsync(CalendarEventRequest request, ...);
    Task<Result<bool>>                        UpdateEventAsync(string externalEventId, CalendarEventRequest request, ...);
    Task<Result<bool>>                        DeleteEventAsync(string externalEventId, ...);
    Task<Result<IReadOnlyList<ExternalCalendarEvent>>> GetEventsAsync(DateTimeOffset from, DateTimeOffset to, ...);
}
```

Register your implementation via the builder returned by `AddServiceScheduling()`:

```csharp
builder.Services
    .AddServiceScheduling()
    .AddPostgreSqlPersistence(connectionString);
    // .AddMyCalendarConnector(options);  ← extend IServiceSchedulingBuilder in your connector package
```

---

## PostgreSQL Persistence

`SchedulingLib.Persistence.PostgreSQL` implements `IStaffMemberRepository`, `IServiceAppointmentRepository`, and `IServiceTypeRepository` using **EF Core for queries** and **Dapper for upserts**.

### Schema

Call `PostgreSqlSchemaInitializer.InitializeAsync` once at startup. It is fully idempotent (`CREATE TABLE IF NOT EXISTS`).

Tables created:

| Table | Key columns |
|---|---|
| `users` | `id`, `name`, `email`, `phone` (CHECK: email OR phone NOT NULL) |
| `service_types` | `id`, `name`, `price NUMERIC(18,4)`, `duration_ticks BIGINT` |
| `staff_members` | `id`, `name`, `email`, `profile_picture_url`, `schedule JSONB`, `offered_services JSONB`, `gallery JSONB` |
| `service_appointments` | `id`, `staff_member_id`, `client_id` (FK → `users`), `service_type_id` (FK → `service_types`), `date`, `time_slot_start/end`, `status` |

### Registration

```csharp
builder.Services
    .AddUserScheduling()
    .AddPostgreSqlPersistence(connectionString);

builder.Services
    .AddServiceScheduling()
    .AddPostgreSqlPersistence(connectionString);

await PostgreSqlSchemaInitializer.InitializeAsync(connectionString);
```

### Connection string format (Npgsql)

```
Host=localhost;Port=5432;Database=scheduling;Username=postgres;Password=secret;
```
