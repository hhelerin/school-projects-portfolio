# Dance School Platform

A multi-tenant SaaS for dance schools. Each school manages their studios, instructors, classes, students, and pricing packages.

---

## Tech Stack

- **.NET 10** — ASP.NET Core MVC (scaffolded controllers and views)
- **Entity Framework Core** — ORM, SQLite database
- **ASP.NET Core Identity** — authentication and authorization
- **MediatR** — CQRS (commands and queries)
- **FluentValidation** — input validation
- **Bootstrap** — UI (scaffolded views)
- **xUnit + Moq** — testing

---

## Solution Structure

```
DanceSchool.sln
├── DanceSchool.Domain          # Entities, interfaces, enums — no dependencies
├── DanceSchool.Application     # CQRS handlers, validators, DTOs, service interfaces
├── DanceSchool.Infrastructure  # EF Core, Identity, repositories, tenant resolution
└── DanceSchool.Web             # ASP.NET Core MVC — controllers, views, DI wiring
```

No separate API project. All user interaction goes through MVC controllers and scaffolded views.

---

## Architecture

Clean Architecture. Dependency rule: Web → Application → Domain. Infrastructure implements interfaces defined in Application. Domain has zero external dependencies.

**Keep It Simple.** No over-engineering. If a straightforward approach works, use it. Avoid abstractions that don't earn their complexity at this scale.

---

## Domain Conventions

All business entities inherit `BaseEntity`:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

Tenant-scoped entities additionally carry `Guid CompanyId`.

**Soft deletes only.** No hard deletes on business entities. EF Core global query filters exclude `IsDeleted = true` records automatically.

---

## Multi-Tenancy

- **Path-based routing**: `localhost/school-name/...`
- **Data isolation**: all queries filter by `CompanyId`
- **Tenant resolution**: `TenantResolutionMiddleware` reads the slug from the URL and populates `ITenantContext` (CompanyId + Slug) for the request lifetime
- **Global query filters** on `AppDbContext` apply `IsDeleted = false` and `CompanyId == currentTenant` to all tenant-scoped entities

---

## Authentication & Authorization

ASP.NET Core Identity. Role-based via `[Authorize(Roles = "...")]`.

**System roles** (platform-level):
- `SystemAdmin` — full platform access, can impersonate company users
- `SystemSupport` — read-only access to any tenant for troubleshooting
- `SystemBilling` — manages subscription plans for packages and payment status

**Company roles** (tenant-level):
- `CompanyOwner` — full control within their tenant
- `CompanyAdmin` — manages users, roles, all operational data
- `CompanyManager` — CRUD on operational data, can view reports
- `CompanyEmployee` — front desk; check-ins, package sales, limited edits

Users can belong to multiple companies. Role assignments are per-company (scoped by CompanyId).

---

## Data Access

- `IRepository<T>` and `IUnitOfWork` interfaces defined in Application
- `Repository<T>` and `UnitOfWork` implemented in Infrastructure
- EF Core CLI for all migrations (`dotnet ef migrations add`, `dotnet ef database update`)
- `AppDbContext` inherits `IdentityDbContext<AppUser>`

---

## CQRS Pattern

MediatR. One file per command/query handler. Kept flat — no excessive folder nesting.

```
Application/
  Features/
    Students/
      Queries/GetStudentProfile.cs
      Commands/RegisterStudent.cs
    Packages/
      Commands/SellPackage.cs
      ...
```

Handlers return `Result<T>`. No exceptions thrown for business logic errors.

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
}
```

---

## Validation

FluentValidation. Every command has a corresponding validator. Validators registered automatically via DI assembly scan.

---

## Error Handling

- Business errors: `Result<T>` pattern — handlers return errors, never throw
- Unexpected exceptions: global exception handler middleware returns a user-friendly error page
- Controller actions check `Result.IsSuccess` and return appropriate views or `ModelState` errors

---

## Key Entities

| Entity                       | Notes                                                       |
|------------------------------|-------------------------------------------------------------|
| `Company`                    | Tenant. Has Slug (URL prefix), IsActive                     |
| `CompanySettings`            | Key-value config per tenant (thresholds, toggles)           |
| `Studio`                     | Physical room belonging to a company                        |
| `StudioRoom`                 | Room within a studio                                        |
| `Feature`                    | Physical features (sprung floor, mirrors, barres, etc.)     |
| `StudioFeature`              | Studio ↔ Feature join, with Valid_From / Valid_Until        |
| `DanceStyle`                 | e.g. Ballet, Hip-Hop, Salsa — scoped per company            |
| `Level`                      | Skill level — scoped per company                            |
| `Instructor`                 | Instructor profile, linked to AppUser                       |
| `CompanyInstructor`          | Company ↔ Instructor join                                   |
| `Class`                      | Recurring class template (style, level, room, instructor)   |
| `ClassSchedule`              | Individual occurrence of a Class (date, start/end time)     |
| `ClassAttendance`            | Check-in record for a student in a ClassSchedule            |
| `Student`                    | Student profile, optionally linked to AppUser               |
| `PackageType`                | Template: BillingCycle, MaxAttendances, ValidityDays, DanceStyleID (nullable), Price |
| `Package`                    | Student's purchased package: StartDate, ExpiryDate, RemainingClasses |
| `TrialRecord`                | One per student per DanceStyle — enforces free trial policy |
| `ShowcaseEligibility`        | Per student per DanceStyle: IsEligible, EvaluatedAt         |
| `StudentShowcaseEligibility` | Student ↔ ShowcaseEligibility join   
| `AppUser`                    | Extends IdentityUser — FirstName, LastName, PreferredCompanyId |
| `CompanyUser`                | AppUser ↔ Company join — IsActive, JoinedAt                 |
| `CompanyUserRole`            | CompanyUser ↔ CompanyRole join                              |
| `CompanyRole`                | Role definition per tenant — IsDefault, IsSystemProtected   |

---

## Business Rules (enforce in Application layer)

- **Trial policy**: one free class per Student per DanceStyle, tracked via TrialRecord. Checked at check-in.
- **Package expiry**: at check-in, packages past ExpiryDate are blocked. Staff see the reason and are prompted to sell a new package.
- **Low-class alert**: when RemainingClasses ≤ threshold (configurable in CompanySettings, default 2), show a warning at check-in.
- **Showcase eligibility**: recalculated after every check-in. Thresholds stored in CompanySettings per style.
- **Soft delete**: calling delete on any business entity sets IsDeleted = true. Global query filters exclude these records.

---

## Database

SQLite for development. One `.db` file, no installation required.

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=danceschool.db"
}
```

EF Core SQLite provider. Migrations via CLI.

---

## Testing

xUnit + Moq. Unit tests target Application layer handlers and domain logic. Infrastructure and Web layers tested with integration tests where it adds value. Test projects mirror the layer they test:

```
DanceSchool.Domain.Tests
DanceSchool.Application.Tests
DanceSchool.Web.Tests
```

---

## What This Project Is Not

- No REST API layer
- No separate frontend framework (React, Angular, etc.)
- No Docker, no cloud deployment
- No background jobs or message queues
- No real payment processing
- No sending e-mails

This is a practice project demonstrating multi-tenant SaaS architecture with Clean Architecture and CQRS in .NET 10.