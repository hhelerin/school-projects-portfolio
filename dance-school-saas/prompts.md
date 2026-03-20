Using spec-driven development and OpenSpec.  

Using Claude Sonnet 4.6 Extended Chat to specify project requirements, 
validating ERD diagram and drafting prompts for /opsx-new.md.  

Using Kilo Code with free default models like MiniMax M2.5, Kimi K2.5 and 
Giga Potato Thinking. Also, Grok-Code-Fast 1 via aI-proxy.cm.itcollege.ee.

Developing in phases where each phase includes:
1. /opsx-new.md [description]
2. /opsx-ff.md  
   2.1 review of generated specs
3. /opsx-apply.md  
   3.1 code review and fixes  
   3.2 running tests  
   3.3 debugging  
   3.4 verifying created features  
   3.5 questioning everything  
4. /opsx-archive.md

# Phase 1

## Phase 1 Step 1

**Project scaffolding, Tenant resolution**
<details>
  <summary>Click to see full prompt</summary>

/opsx-new.md  
Change name: project-scaffold

Scaffold the Dance School Platform foundation on top of the existing
WebApp2025 solution structure. Do NOT create new projects or rename
existing ones. Work within what exists.

See openspec/project.md and .kilocode/config.yaml for full conventions.

--- PROJECT MAPPING ---

This solution uses a pre-existing layer structure. Map our architecture
to it as follows:

App.Domain      → Domain layer
Entities, enums, domain interfaces
Must have ZERO external NuGet dependencies

App.DAL.EF      → Infrastructure layer
EF Core AppDbContext, migrations, repositories
ASP.NET Core Identity configuration

App.BLL         → Application layer
MediatR handlers (commands + queries)
FluentValidation validators
Service interfaces and implementations

App.DTO         → DTOs and ViewModels
Input models, output models, MediatR request/response types

App.Helpers     → Cross-cutting concerns
Result<T>, ITenantContext, ICurrentUserService,
TenantResolutionMiddleware, extension methods

App.Resources   → String resources (leave existing structure intact)

Base.Resources  → Leave entirely untouched

WebApp          → MVC layer
Controllers, Razor views, Program.cs, DI wiring

WebApp.Tests    → xUnit + Moq tests

--- WHAT TO BUILD ---

App.Domain
- BaseEntity:
  public Guid Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public bool IsDeleted { get; set; }
- ITenantEntity marker interface — adds: public Guid CompanyId { get; set; }
- IRepository<T> interface (generic CRUD + soft delete)
- IUnitOfWork interface
- Enums:
  SubscriptionTier { Free, Standard, Premium }
  BillingCycle { DropIn, ClassCard, MonthlyStyle, MonthlyAll }
  CompanyRoleType { SystemAdmin, SystemSupport, SystemBilling,
  CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee }
- Core entities (plain C# classes, no EF attributes):
  Company: Id, Name, Slug, IsActive, SubscriptionTier, RegistrationCode?,
  VATCode?, ContactInfo?, Details?, CreatedAt, UpdatedAt, IsDeleted
  CompanySettings: Id, CompanyId, Key, Value

App.Helpers
- Result<T> and non-generic Result:
  bool IsSuccess, T? Value, string? Error
  static Result<T> Success(T value)
  static Result<T> Failure(string error)
- ITenantContext interface: Guid CompanyId, string Slug
- ICurrentUserService interface: Guid UserId, Guid CompanyId, string[] Roles
- TenantResolutionMiddleware:
  reads slug from URL path position 1 (/school-name/...)
  resolves CompanyId from database by slug
  populates ITenantContext scoped service for request lifetime
  returns 404 if slug not found or company inactive

App.DAL.EF
- AppUser extending IdentityUser:
  FirstName, LastName, PreferredCompanyId (Guid?)
- CompanyUser join: AppUser ↔ Company
  Id, AppUserId, CompanyId, IsActive, JoinedAt (DateTime UTC)
- CompanyRole:
  Id, Name, Description, CompanyId, IsDefault (bool),
  IsSystemProtected (bool), SortOrder (int)
- CompanyUserRole join: CompanyUser ↔ CompanyRole
  Id, CompanyUserId, CompanyRoleId, IsActive (bool)
- AuditLog (infrastructure-only, does NOT inherit BaseEntity):
  Id, CompanyId, UserId, EntityType, EntityId, Action,
  OldValues (string?), NewValues (string?), Timestamp (DateTime UTC)
- AppDbContext inheriting IdentityDbContext<AppUser>:
  Global query filters: IsDeleted = false AND CompanyId == currentTenant
  applied to all ITenantEntity types
  SaveChangesAsync override: auto-set CreatedAt/UpdatedAt on add/modify
  and auto-apply CompanyId from ITenantContext
  All DateTime stored as UTC via global ValueConverter
  Cascade delete disabled globally in OnModelCreating
- Repository<T> implementing IRepository<T>
- UnitOfWork implementing IUnitOfWork
- InitialCreate migration covering:
  AuditLog, AppUser (extended), Company, CompanySettings,
  CompanyUser, CompanyRole, CompanyUserRole,
  all ASP.NET Core Identity tables

App.BLL
- Empty Features/ folder, structured for:
  Features/{EntityName}/Queries/
  Features/{EntityName}/Commands/
- No handlers yet — those come in subsequent changes

App.DTO
- Empty, ready for Change 2 onwards

WebApp — Program.cs
- EF Core with SQLite:
  "Data Source=danceschool.db"
- ASP.NET Core Identity (AppUser)
- MediatR scanning App.BLL assembly
- FluentValidation scanning App.BLL assembly
- TenantContext registered as scoped ITenantContext
- CurrentUserService registered as scoped ICurrentUserService
- Repository<T> and UnitOfWork registered
- Middleware pipeline order:
  UseStaticFiles
  TenantResolutionMiddleware (custom, from App.Helpers)
  UseAuthentication
  UseAuthorization
- Global exception handler returning Views/Shared/Error.cshtml

WebApp — Views and Controllers
- _Layout.cshtml with Bootstrap navbar (placeholder links only)
- HomeController with Index action
- Home/Index.cshtml — simple landing page

WebApp.Tests
- xUnit and Moq packages referenced
- One placeholder test class confirming the test project runs

--- REQUIRED NUGET PACKAGES ---

App.DAL.EF:
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore

App.BLL:
MediatR
FluentValidation
FluentValidation.DependencyInjectionExtensions

WebApp:
Microsoft.EntityFrameworkCore.Design (for migrations CLI)

WebApp.Tests:
xUnit
Moq
Microsoft.NET.Test.Sdk

--- MIGRATIONS (manual steps after apply) ---

dotnet ef migrations add InitialCreate \
--project App.DAL.EF \
--startup-project WebApp

dotnet ef database update \
--project App.DAL.EF \
--startup-project WebApp

--- DO NOT ---
- Create new .csproj files or solution folders
- Rename any existing projects
- Touch Base.Resources or App.Resources internals
- Add API controllers or [ApiController] attributes
- Add JWT, Swagger, or Docker configuration
- Add any application feature logic — that starts in Change 2
</details>

## Phase 1 Step 2

**Identity and Auth**  

 <details>
  <summary>Click to see full prompt</summary>
/opsx-new.md  

Change name: identity-and-auth

Build login, logout, and password reset for the Dance School Platform.
This is Change 2 of 13. The project scaffold (BaseEntity, AppDbContext,
AppUser, Identity configuration, SQLite, middleware pipeline) is already
built and archived. Do not recreate anything from project-scaffold.

See openspec/project.md and .kilocode/config.yaml for full conventions.
Solution structure: App.Domain, App.BLL, App.DAL.EF, App.DTO,
App.Helpers, WebApp, WebApp.Tests.

--- WHAT TO BUILD ---

App.DTO
- LoginInputModel: Email, Password, RememberMe
- ForgotPasswordInputModel: Email
- ResetPasswordInputModel: Email, Token, NewPassword, ConfirmPassword

WebApp — AccountController
- GET  /login              → Login view
- POST /login              → validate LoginInputModel, sign in via
  SignInManager, redirect to tenant home
  or /login?error on failure
- POST /logout             → sign out, redirect to /login
- GET  /forgot-password    → ForgotPassword view
- POST /forgot-password    → generate reset token, log it to console
  (no email service yet — log token so it
  can be used manually in development)
- GET  /reset-password     → ResetPassword view (reads token from query)
- POST /reset-password     → validate ResetPasswordInputModel,
  call UserManager.ResetPasswordAsync,
  redirect to /login on success

WebApp — Views/Account/
- Login.cshtml             — email + password form, Bootstrap, show
  error message on failed login
- ForgotPassword.cshtml    — email form
- ResetPasswordConfirmation.cshtml — success message with link to login
- ResetPassword.cshtml     — new password + confirm password form,
  hidden token and email fields

WebApp — Layout & Nav
- Update _Layout.cshtml navbar:
  If authenticated: show user's FirstName + LastName, Logout button
  If not: show Login link
- Add [Authorize] to HomeController so unauthenticated users are
  redirected to /login

WebApp — Program.cs additions
- Configure Identity options:
  RequireConfirmedAccount = false
  Password: MinLength 8, require digit, require uppercase
- Set login path: /login
- Set access denied path: /access-denied
- Add a simple AccessDenied view (Views/Shared/AccessDenied.cshtml)

--- DO NOT ---
- Build registration — that comes in Change 3 (tenant-onboarding)
- Build any company or tenant selection UI — Change 3
- Add email sending — log the reset token to console/logger only
- Build any role management screens
- Touch anything created in project-scaffold

--- TESTS (WebApp.Tests) ---
- AccountController login: valid credentials redirect to home
- AccountController login: invalid credentials return login view
  with error
- AccountController logout: signs out and redirects to /login
</details>

# Phase 2

## Phase 2 Step 1

**School Registration and Staff Management**

 <details>
  <summary>Click to see full prompt</summary>

/opsx-new.md  

Change name: tenant-onboarding

Build school self-service registration and staff invitation.
This is Change 3 of 13. project-scaffold and identity-and-auth
are built and archived. Do not recreate anything from those changes.

See openspec/project.md and .kilocode/config.yaml for full conventions.

--- WHAT TO BUILD ---

SCHOOL SELF-SERVICE REGISTRATION
A new dance school owner visits /register and signs up.
Submitting the form must:
1. Create a new Company record (IsActive = true, SubscriptionTier = Free)
2. Create an AppUser for the owner
3. Create a CompanyUser join record (IsActive = true, JoinedAt = now)
4. Assign the CompanyOwner role to that CompanyUser
5. Sign the user in automatically
6. Redirect to /school-name/dashboard (stub page is fine)

App.DTO
- RegisterSchoolInputModel:
  OwnerFirstName, OwnerLastName, Email, Password, ConfirmPassword,
  SchoolName, Slug (URL-friendly, auto-generated from SchoolName
  but editable by user), RegistrationCode (string, optional),
  VATCode (string, optional)

App.BLL
- RegisterSchoolCommand + handler:
  Validates slug is unique (case-insensitive)
  Validates email not already in use
  Executes all 4 creation steps in a single transaction via IUnitOfWork
  Returns Result<RegisterSchoolResult> with CompanyId and Slug

WebApp — RegistrationController
- GET  /register    → registration form view
- POST /register    → dispatch RegisterSchoolCommand, sign in,
  redirect to /{slug}/dashboard on success,
  return form with errors on failure

WebApp — Views/Registration/
- Register.cshtml   — school + owner fields, slug preview,
  Bootstrap, client-side slug auto-generation
  from SchoolName input (simple JS, spaces→hyphens,
  lowercase)

STAFF INVITATION
A CompanyAdmin invites a new staff member by email and assigns them
a company role.

App.DTO
- InviteStaffInputModel: Email, FirstName, LastName, CompanyRoleId
- StaffInvitationViewModel: list of CompanyRoles for the dropdown

App.BLL
- InviteStaffCommand + handler:
  If AppUser with that email already exists: create CompanyUser
  and CompanyUserRole for this company only
  If AppUser does not exist: create AppUser with a temporary
  random password, create CompanyUser + CompanyUserRole,
  log the temp password to console (no email service yet)
  Returns Result<InviteStaffResult> with UserId

WebApp — StaffController (scoped under /{slug}/)
- GET  /{slug}/staff/invite   → invite form, populate role dropdown
  from CompanyRoles for this tenant
- POST /{slug}/staff/invite   → dispatch InviteStaffCommand,
  redirect to /{slug}/staff on success

WebApp — Views/Staff/
- Invite.cshtml — email, name fields, role dropdown, Bootstrap

WebApp — Dashboard stub
- DashboardController with GET /{slug}/dashboard
- Views/Dashboard/Index.cshtml — plain page: "Welcome, {FirstName}.
  Your school {SchoolName} is set up." with placeholder nav links

--- AUTHORIZATION ---
- /register is anonymous
- /{slug}/staff/invite requires CompanyAdmin or CompanyOwner role
- /{slug}/dashboard requires any authenticated CompanyUser

--- VALIDATION ---
- Slug: lowercase letters, numbers, hyphens only; 3–64 chars; unique
- Password: matches Identity options set in Change 2 (min 8, digit,
  uppercase)
- Email: valid format, unique for registration

--- DO NOT ---
- Build user management screens (list, edit, deactivate staff)
- Build company settings configuration
- Build any email sending — console/logger only
- Build role management CRUD
- Touch identity-and-auth or project-scaffold code

--- TESTS (WebApp.Tests) ---
- RegisterSchoolCommand: valid input creates company, user,
  CompanyUser, and assigns CompanyOwner role
- RegisterSchoolCommand: duplicate slug returns failure result
- RegisterSchoolCommand: duplicate email returns failure result
- InviteStaffCommand: existing AppUser gets CompanyUser record,
  not a duplicate AppUser
 </details>


## Phase 2 Step 2

**School Configuration**

 <details>
  <summary>Click to see full prompt</summary>

/opsx-new.md  

Change name: school-configuration

Build CRUD management for a school's core configuration:
dance styles, levels, studios, rooms, features, and instructors.
This is Change 4 of 13. project-scaffold, identity-and-auth, and
tenant-onboarding are built and archived. Do not recreate anything
from those changes.

See openspec/project.md and .kilocode/config.yaml for full conventions.
All controllers are scoped under /{slug}/. All data is tenant-isolated
via CompanyId. Follow the same controller/view/handler pattern
established in previous changes.

--- ENTITIES (App.Domain) ---

All inherit BaseEntity and implement ITenantEntity (carry CompanyId).
Soft delete only — no hard deletes.

DanceStyle
- Id, CompanyId, Name (string), Details (string?), IsDeleted

Level
- Id, CompanyId, Name (string), Details (string?), IsDeleted

Feature  (physical room features: sprung floor, mirrors, barres, etc.)
- Id, Name (string), Details (string?)
- Note: Feature is NOT tenant-scoped — it is a shared reference
  table across all companies. No CompanyId, no ITenantEntity.

Studio
- Id, CompanyId, Name (string), Details (string?),
  Contact_Info (string?), IsDeleted

StudioRoom
- Id, StudioId (FK), Name (string), Details (string?), IsDeleted
- Belongs to a Studio within the same tenant

StudioFeature  (join: StudioRoom ↔ Feature)
- Id, StudioRoomId (FK), FeatureId (FK),
  Valid_From (DateTime? UTC), Valid_Until (DateTime? UTC),
  Details (string?)
- No IsDeleted — remove by deleting the join record

Instructor
- Id, CompanyId, Name (string), PersonalId (string?),
  ContactInfo (string?), AppUserId (Guid?, FK nullable),
  Details (string?), IsDeleted

--- APP.BLL — COMMANDS AND QUERIES ---

Follow flat feature folder structure:
Features/DanceStyles/, Features/Levels/, Features/Studios/,
Features/Instructors/

For each entity, build:

Queries:
GetList{Entity}Query    → returns list for the current tenant
Get{Entity}ByIdQuery    → returns single record or Result.Failure
if not found or wrong tenant

Commands:
Create{Entity}Command   → creates, sets CompanyId from ITenantContext
Update{Entity}Command   → updates, validates belongs to current tenant
Delete{Entity}Command   → soft delete (IsDeleted = true)
Feature has no delete (shared reference)

StudioRoom-specific:
GetRoomsByStudioQuery(StudioId)
AddStudioFeatureCommand(StudioRoomId, FeatureId, Valid_From, Valid_Until)
RemoveStudioFeatureCommand(StudioFeatureId)

All handlers return Result<T>. Validators for every command.

--- APP.DTO ---

For each entity:
- {Entity}ListItemDto     → fields shown in index/list view
- {Entity}DetailDto       → fields shown in detail/edit view
- Create{Entity}InputModel
- Update{Entity}InputModel

--- CONTROLLERS (WebApp) ---

DanceStyleController  /{slug}/dance-styles/
GET    /              → index, list all active styles for tenant
GET    /create        → create form
POST   /create        → dispatch CreateDanceStyleCommand
GET    /{id}/edit     → edit form
POST   /{id}/edit     → dispatch UpdateDanceStyleCommand
POST   /{id}/delete   → dispatch DeleteDanceStyleCommand,
redirect to index

LevelController  /{slug}/levels/
Same CRUD pattern as DanceStyleController

StudioController  /{slug}/studios/
GET    /              → index, list studios with room count
GET    /create        → create form
POST   /create        → dispatch CreateStudioCommand
GET    /{id}          → detail view, shows rooms list
GET    /{id}/edit     → edit form
POST   /{id}/edit     → dispatch UpdateStudioCommand
POST   /{id}/delete   → soft delete studio

StudioRoomController  /{slug}/studios/{studioId}/rooms/
GET    /              → list rooms for this studio
GET    /create        → create form
POST   /create        → dispatch CreateStudioRoomCommand
GET    /{id}/edit     → edit form, includes current features list
and add/remove feature controls
POST   /{id}/edit     → dispatch UpdateStudioRoomCommand
POST   /{id}/add-feature     → dispatch AddStudioFeatureCommand
POST   /{id}/remove-feature  → dispatch RemoveStudioFeatureCommand
POST   /{id}/delete          → soft delete room

InstructorController  /{slug}/instructors/
GET    /              → index, list active instructors
GET    /create        → create form, optional AppUser dropdown
(list CompanyUsers for this tenant)
POST   /create        → dispatch CreateInstructorCommand
GET    /{id}/edit     → edit form
POST   /{id}/edit     → dispatch UpdateInstructorCommand
POST   /{id}/delete   → soft delete

--- VIEWS ---

Follow the same Bootstrap layout established in previous changes.
For each controller:
- Index.cshtml       → Bootstrap table, Name column, Edit + Delete
  buttons, Create New button
- Create.cshtml      → form with validation summary
- Edit.cshtml        → pre-filled form with validation summary

Studios/Detail.cshtml  → studio info + rooms table + link to manage rooms
StudioRooms/Edit.cshtml → room form + current features table with
Remove buttons + Add Feature form (dropdown
of all Features, Valid_From / Valid_Until
date fields)

--- NAVIGATION ---

Update _Layout.cshtml (or Dashboard) to add links under a
"Configuration" nav section (visible to CompanyAdmin and CompanyManager):
- Dance Styles
- Levels
- Studios
- Instructors

--- AUTHORIZATION ---

All /{slug}/ routes require authenticated CompanyUser for this tenant.
Create, Edit, Delete actions require CompanyAdmin or CompanyManager.
Index and detail views require any authenticated CompanyUser.

--- EF CORE ---

Add EF Core configurations (IEntityTypeConfiguration<T>) for all
new entities in App.DAL.EF:
- Indexes on CompanyId for all tenant-scoped entities
- Index on Instructor.AppUserId
- StudioRoom FK to Studio (restrict delete — cannot delete studio
  that has rooms)
- StudioFeature FK to StudioRoom and Feature
- Feature table seeded with common values:
  Sprung Floor, Mirrors, Barres, Poles, Aerial Rigging, Sound System

Add EF Core migration named "SchoolConfiguration"

--- DO NOT ---
- Build class scheduling — that is Change 5
- Build style-level compatibility constraints — deferred post-MVP
- Build instructor-style qualification tracking — deferred post-MVP
- Build studio-feature compatibility enforcement — deferred post-MVP
- Add any billing or subscription checks yet

--- TESTS (WebApp.Tests) ---
- CreateDanceStyleCommand: creates style scoped to correct tenant
- DeleteDanceStyleCommand: sets IsDeleted, does not hard delete
- GetListDanceStyleQuery: only returns styles for current tenant
- AddStudioFeatureCommand: creates join record with correct dates
- RemoveStudioFeatureCommand: removes join record
- CreateInstructorCommand: valid input creates instructor under tenant
</details>

## Phase 2 Step 3

**Class scheduling**

 <details>
  <summary>Click to see full prompt</summary>

/opsx-new.md  

Change name: class-scheduling

Build class template creation, recurring schedule generation,
single-instance editing, cancellation, and the schedule view.
This is Change 5 of 13. project-scaffold, identity-and-auth,
tenant-onboarding, and school-configuration are built and archived.
Do not recreate anything from those changes.

See openspec/project.md and .kilocode/config.yaml for full conventions.
All controllers scoped under /{slug}/. All data tenant-isolated via
CompanyId. Follow established controller/view/handler patterns.

--- ENTITIES (App.Domain) ---

All inherit BaseEntity and implement ITenantEntity unless noted.

Class  (recurring class template)
- Id, CompanyId, Name (string), Details (string?)
- StudioRoomId (Guid, FK)
- InstructorId (Guid, FK)
- DanceStyleId (Guid, FK)
- LevelId (Guid, FK)
- Capacity (int)
- IsDeleted

ClassSchedule  (individual occurrence of a Class)
- Id, CompanyId, ClassId (Guid, FK)
- Date (DateOnly)
- StartTime (TimeOnly)
- EndTime (TimeOnly)
- StudioRoomId (Guid, FK)  ← may differ from Class default (room swap)
- IsCancelled (bool, default false)
- CancellationReason (string?)
- IsException (bool)  ← true when this occurrence was manually edited
- Details (string?)
- IsDeleted

RecurrencePattern  (owned by Class, not a separate table)
Define as owned entity on Class:
- DayOfWeek (DayOfWeek enum)
- StartTime (TimeOnly)
- EndTime (TimeOnly)
- RecurrenceStartDate (DateOnly)
- RecurrenceEndDate (DateOnly)

--- APP.BLL --- COMMANDS AND QUERIES ---

Features/Classes/, Features/ClassSchedules/

CLASS COMMANDS AND QUERIES:

CreateClassCommand
Input: Name, Details, StudioRoomId, InstructorId, DanceStyleId,
LevelId, Capacity, RecurrencePattern (DayOfWeek, StartTime,
EndTime, RecurrenceStartDate, RecurrenceEndDate)
Logic:
1. Validate all FK references belong to current tenant
2. Validate RecurrenceEndDate > RecurrenceStartDate
3. Create Class record with owned RecurrencePattern
4. Generate ClassSchedule occurrences for every matching
DayOfWeek between RecurrenceStartDate and RecurrenceEndDate
5. For each generated occurrence run conflict check (see below)
6. If any conflicts found return Result.Failure listing conflicts
— do not partial-save
7. Save Class + all ClassSchedule records in one transaction
Returns: Result<Guid> (ClassId)

UpdateClassCommand
Input: ClassId, Name, Details, Capacity only
Note: changing schedule pattern or room is out of scope —
use EditClassScheduleInstanceCommand for per-occurrence changes
Returns: Result

DeleteClassCommand
Soft delete Class. Also soft delete all future ClassSchedule
occurrences (Date >= today) that are not already cancelled.
Past occurrences are untouched (attendance history).
Returns: Result

GetClassByIdQuery      → ClassDetailDto
GetClassListQuery      → list of ClassListItemDto for current tenant

CLASS SCHEDULE COMMANDS AND QUERIES:

EditClassScheduleInstanceCommand
Input: ClassScheduleId, Date, StartTime, EndTime, StudioRoomId,
Details, CancellationReason (nullable)
Logic:
1. Validate ClassSchedule belongs to current tenant
2. Run conflict check for new room/time if changed
3. If conflict found return Result.Failure
4. Set IsException = true
5. Update fields
Returns: Result

CancelClassScheduleInstanceCommand
Input: ClassScheduleId, CancellationReason (string)
Logic:
1. Validate belongs to current tenant
2. Set IsCancelled = true, CancellationReason
3. Do NOT soft delete — cancelled records must remain visible
Returns: Result

GetScheduleByWeekQuery
Input: WeekStartDate (DateOnly), filters: DanceStyleId?, LevelId?,
StudioRoomId?, InstructorId? (all optional)
Returns: list of ClassScheduleListItemDto for the requested week,
including cancelled occurrences (marked separately)

GetClassSchedulesByClassQuery(ClassId)
Returns: all future occurrences for a class template

CONFLICT CHECK (private method in CreateClass and EditInstance handlers)
A conflict exists when:
Same StudioRoomId AND same Date AND time ranges overlap
(new StartTime < existing EndTime AND new EndTime > existing StartTime)
AND existing ClassSchedule IsDeleted = false AND IsCancelled = false
Return list of conflicting ClassSchedule Ids and their times.

--- APP.DTO ---

ClassListItemDto
- Id, Name, DanceStyleName, LevelName, InstructorName,
  StudioRoomName, Capacity, RecurrenceSummary (string,
  e.g. "Tuesdays 18:00–19:00")

ClassDetailDto
- All ClassListItemDto fields plus Details, RecurrencePattern fields,
  upcoming ClassSchedule count

ClassScheduleListItemDto
- Id, ClassId, ClassName, Date, StartTime, EndTime,
  StudioRoomName, InstructorName, DanceStyleName, LevelName,
  IsCancelled, IsException, CancellationReason

CreateClassInputModel
- Name, Details, StudioRoomId, InstructorId, DanceStyleId, LevelId,
  Capacity, DayOfWeek, StartTime, EndTime,
  RecurrenceStartDate, RecurrenceEndDate
- Dropdowns populated from current tenant's rooms, instructors,
  styles, levels

EditClassScheduleInstanceInputModel
- ClassScheduleId, Date, StartTime, EndTime, StudioRoomId, Details

CancelClassScheduleInstanceInputModel
- ClassScheduleId, CancellationReason

--- CONTROLLERS (WebApp) ---

ClassController  /{slug}/classes/
GET    /               → index, list all class templates for tenant
GET    /create         → create form, populate all dropdowns
POST   /create         → dispatch CreateClassCommand,
on conflict show conflict details in view,
redirect to index on success
GET    /{id}           → detail view, shows upcoming schedule list
GET    /{id}/edit      → edit form (name, details, capacity only)
POST   /{id}/edit      → dispatch UpdateClassCommand
POST   /{id}/delete    → dispatch DeleteClassCommand,
redirect to index

ClassScheduleController  /{slug}/schedule/
GET    /               → weekly schedule view, default to current week
query params: weekStart, styleId, levelId,
roomId, instructorId
GET    /{id}/edit      → edit single occurrence form
POST   /{id}/edit      → dispatch EditClassScheduleInstanceCommand,
on conflict show conflict message
POST   /{id}/cancel    → dispatch CancelClassScheduleInstanceCommand,
redirect back to schedule view

--- VIEWS ---

Classes/Index.cshtml
Bootstrap table: Name, Style, Level, Instructor, Room, Recurrence,
Capacity. Edit + Delete buttons. Create New button.

Classes/Create.cshtml
Form with all fields. Dropdowns for Room, Instructor, Style, Level.
DayOfWeek as select. StartTime and EndTime as time inputs.
RecurrenceStartDate and RecurrenceEndDate as date inputs.
If conflict returned from handler, show conflict detail table
(conflicting date, time, class name) above form.

Classes/Detail.cshtml
Class info header. Table of upcoming ClassSchedule occurrences:
Date, Time, Room, IsCancelled (badge), IsException (badge).
Edit and Cancel buttons per row.

Classes/Edit.cshtml
Form: Name, Details, Capacity only. Note to user that schedule
changes must be done per-occurrence from the schedule view.

Schedule/Index.cshtml
Weekly calendar-style layout (Mon–Sun columns, time rows OR simple
Bootstrap table grouped by day — keep it simple).
Each entry shows: ClassName, Style, Level, Instructor, Room,
start–end time. Cancelled entries shown with strikethrough + badge.
Exception entries shown with an "edited" badge.
Filter bar at top: week navigation (prev/next), Style dropdown,
Level dropdown, Room dropdown, Instructor dropdown.
Edit and Cancel buttons on each entry (CompanyManager+ only).

Schedule/Edit.cshtml
Form: Date, StartTime, EndTime, StudioRoom dropdown, Details.
Show original values for reference.
If conflict returned, show conflict detail above form.

--- NAVIGATION ---

Update _Layout.cshtml or Dashboard nav to add under an "Operations"
section (visible to CompanyManager and above):
- Classes
- Schedule

--- AUTHORIZATION ---

All /{slug}/ routes: authenticated CompanyUser for this tenant.
Create, Edit, Delete, Cancel: CompanyManager or above.
Index, Detail, Schedule view: any authenticated CompanyUser.

--- EF CORE ---

IEntityTypeConfiguration<T> for Class and ClassSchedule in App.DAL.EF:
- Class: index on CompanyId, index on StudioRoomId
- ClassSchedule: index on CompanyId, index on ClassId,
  index on (StudioRoomId, Date) for conflict queries
- RecurrencePattern as owned entity on Class (no separate table)
- ClassSchedule FK to Class: restrict delete
  (use DeleteClassCommand soft delete instead)
- ClassSchedule FK to StudioRoom: restrict delete
- TimeOnly and DateOnly stored as TEXT in SQLite
  (add ValueConverter for both types globally in AppDbContext)

Add EF Core migration named "ClassScheduling"

--- DO NOT ---
- Build student enrollment or check-in — that is Change 8
- Build attendance tracking — Change 8
- Build waitlists — out of scope for MVP
- Build email notifications for cancellations — out of scope for MVP
- Add capacity enforcement — that comes in Change 8 at check-in

--- TESTS (WebApp.Tests) ---
- CreateClassCommand: generates correct number of occurrences
  for a 4-week recurrence on a given DayOfWeek
- CreateClassCommand: returns failure when conflict exists,
  does not save any records
- CreateClassCommand: saves class and all occurrences in one
  transaction when no conflicts
- EditClassScheduleInstanceCommand: sets IsException = true
- EditClassScheduleInstanceCommand: returns failure on conflict
- CancelClassScheduleInstanceCommand: sets IsCancelled,
  does not soft delete the record
- DeleteClassCommand: soft deletes future occurrences,
  leaves past occurrences untouched
- GetScheduleByWeekQuery: returns only occurrences for requested
  week, respects tenant isolation
</details>

## Phase 2 Step 4 

**Student Management**

<details>
  <summary>Click to see full prompt</summary>

/opsx-new.md  
Change name: student-management

Build student registration, profile view, and editing.
This is Change 6 of 13. All previous changes (project-scaffold,
identity-and-auth, tenant-onboarding, school-configuration,
class-scheduling) are built and archived. Do not recreate anything
from those changes.

See openspec/project.md and .kilocode/config.yaml for full conventions.
All controllers scoped under /{slug}/. All data tenant-isolated via
CompanyId. Follow established controller/view/handler patterns.

--- ENTITIES (App.Domain) ---

Student
- Id, CompanyId, Name (string), PersonalId (string?),
  ContactInfo (string?), Details (string?)
- AppUserId (Guid?, FK nullable) — a student may or may not
  have a platform login account. Walk-in students registered
  by staff will not have one.
- IsDeleted

No other new entities in this change. TrialRecord, Package,
and ShowcaseEligibility are referenced on the student profile
view as empty placeholder sections only — they are built in
Changes 7, 8, and 9 respectively.

--- APP.BLL --- COMMANDS AND QUERIES ---

Features/Students/

RegisterStudentCommand
Input: Name, PersonalId (optional), ContactInfo (optional),
Details (optional), AppUserId (optional)
Logic:
1. If AppUserId provided, validate that AppUser exists and
is not already a Student in this tenant
2. Duplicate detection: if a Student with the same Name AND
ContactInfo already exists in this tenant, return
Result.Failure with a clear duplicate warning message
(do not hard-block — return warning so staff can
confirm or override via ConfirmDuplicate flag)
ConfirmDuplicate (bool, default false) — if true, bypass
duplicate check and save anyway
Sets CompanyId from ITenantContext
Returns: Result<Guid> (StudentId)

UpdateStudentCommand
Input: StudentId, Name, PersonalId, ContactInfo, Details,
AppUserId (nullable)
Logic:
1. Validate Student belongs to current tenant
2. If AppUserId changed, validate new AppUser exists and
is not already linked to another Student in this tenant
Returns: Result

DeleteStudentCommand
Soft delete only (IsDeleted = true)
Do not allow delete if Student has any active Package records
(active = not expired, not deleted) — return Result.Failure
with message "Student has active packages. Deactivate packages
before removing the student."
Returns: Result

GetStudentListQuery
Input: SearchTerm (string?, searches Name and ContactInfo),
PageNumber (int, default 1), PageSize (int, default 20)
Returns: PagedResult<StudentListItemDto>
Only returns IsDeleted = false students for current tenant

GetStudentByIdQuery
Input: StudentId
Validates belongs to current tenant
Returns: Result<StudentDetailDto>

PagedResult<T> helper (add to App.Helpers):
Items (List<T>), PageNumber, PageSize, TotalCount, TotalPages,
HasPreviousPage, HasNextPage

--- APP.DTO ---

StudentListItemDto
- Id, Name, ContactInfo, HasAppUser (bool),
  ActivePackageCount (int — count of non-expired, non-deleted
  packages, computed in query), CreatedAt

StudentDetailDto
- Id, Name, PersonalId, ContactInfo, Details
- AppUserId (Guid?), AppUserFullName (string?)
- ActivePackages (List<PackageSummaryDto>) — empty list for now,
  populated in Change 7
- TrialRecords (List<TrialRecordSummaryDto>) — empty list for now,
  populated in Change 8
- ShowcaseEligibilities (List<ShowcaseEligibilitySummaryDto>)
  — empty list for now, populated in Change 9
- AttendanceCount (int) — count of ClassAttendance records,
  always 0 for now, populated in Change 8
- CreatedAt, UpdatedAt

PackageSummaryDto        — placeholder, empty class, no properties yet
TrialRecordSummaryDto    — placeholder, empty class, no properties yet
ShowcaseEligibilitySummaryDto — placeholder, empty class, no properties yet

RegisterStudentInputModel
- Name, PersonalId, ContactInfo, Details, AppUserId (Guid?)
- ConfirmDuplicate (bool, default false, hidden field)

UpdateStudentInputModel
- StudentId, Name, PersonalId, ContactInfo, Details,
  AppUserId (Guid?)

--- CONTROLLERS (WebApp) ---

StudentController  /{slug}/students/

GET    /
→ dispatch GetStudentListQuery with SearchTerm and page params
→ Students/Index.cshtml

GET    /create
→ Students/Create.cshtml
→ populate optional AppUser dropdown: CompanyUsers in this
tenant who are not already linked to a Student

POST   /create
→ dispatch RegisterStudentCommand (ConfirmDuplicate = false)
→ if Result.Failure and error is duplicate warning:
return Create view with duplicate warning banner and
a "Save Anyway" button that resubmits with
ConfirmDuplicate = true
→ if other failure: return Create view with ModelState error
→ on success: redirect to /{slug}/students/{id}

GET    /{id}
→ dispatch GetStudentByIdQuery
→ Students/Detail.cshtml

GET    /{id}/edit
→ dispatch GetStudentByIdQuery, map to UpdateStudentInputModel
→ Students/Edit.cshtml

POST   /{id}/edit
→ dispatch UpdateStudentCommand
→ on failure: return Edit view with errors
→ on success: redirect to /{slug}/students/{id}

POST   /{id}/delete
→ dispatch DeleteStudentCommand
→ on failure (active packages): redirect back to detail view
with error message in TempData
→ on success: redirect to /{slug}/students/

--- VIEWS ---

Students/Index.cshtml
Search bar at top (text input + Search button, GET form).
Bootstrap table: Name, ContactInfo, Active Packages count,
Has Login (yes/no badge), Registered date.
Edit and View buttons per row.
Pagination controls at bottom (previous / page numbers / next).
"Register New Student" button top right.

Students/Create.cshtml
Form fields: Name (required), PersonalId, ContactInfo, Details.
Optional AppUser dropdown (label: "Link to platform account").
If duplicate warning returned from handler: show yellow Bootstrap
alert banner with warning message and a "Save Anyway" button.
Normal "Register" submit button.
Hidden ConfirmDuplicate field (false by default, set to true
by "Save Anyway" button via JS).

Students/Detail.cshtml
Student info card: Name, PersonalId, ContactInfo, Details,
linked AppUser (if any), registered date.
Edit button top right. Delete button (with confirmation).
Four placeholder sections as Bootstrap cards below the info card:
"Active Packages" — "Coming soon" placeholder text
"Trial Records"   — "Coming soon" placeholder text
"Attendance"      — "Coming soon" placeholder text
"Showcase Eligibility" — "Coming soon" placeholder text

Students/Edit.cshtml
Pre-filled form. Same fields as Create.
AppUser dropdown — shows currently linked user, allows change.
Save and Cancel buttons.

--- NAVIGATION ---

Update _Layout.cshtml or Dashboard nav to add under "Operations"
section alongside Classes and Schedule:
- Students

--- AUTHORIZATION ---

All /{slug}/students/ routes: authenticated CompanyUser for tenant.
GET / and GET /{id}: any authenticated CompanyUser
(CompanyEmployee, CompanyManager, CompanyAdmin, CompanyOwner)
GET /create, POST /create: CompanyEmployee and above
GET /{id}/edit, POST /{id}/edit: CompanyEmployee and above
POST /{id}/delete: CompanyAdmin or CompanyOwner only

--- EF CORE ---

IEntityTypeConfiguration<Student> in App.DAL.EF:
- Index on CompanyId
- Index on AppUserId
- Index on (CompanyId, Name) for duplicate detection query
- FK to AppUser: set null on AppUser delete (not cascade)
- Restrict delete not needed — soft delete only

Add EF Core migration named "StudentManagement"

--- DO NOT ---
- Build package selling or package display — Change 7
- Build trial record logic — Change 8
- Build attendance display — Change 8
- Build showcase eligibility — Change 9
- Build student self-registration or portal — out of scope for MVP
- Build bulk import of students — out of scope for MVP

--- TESTS (WebApp.Tests) ---
- RegisterStudentCommand: valid input creates student under
  correct tenant
- RegisterStudentCommand: returns duplicate warning when Name
  and ContactInfo match existing student in same tenant
- RegisterStudentCommand: ConfirmDuplicate = true bypasses
  duplicate check and saves
- RegisterStudentCommand: does not flag duplicate for same
  Name + ContactInfo in a DIFFERENT tenant
- UpdateStudentCommand: returns failure if AppUserId already
  linked to another student in same tenant
- DeleteStudentCommand: soft deletes student with no active packages
- DeleteStudentCommand: returns failure when student has
  active packages
- GetStudentListQuery: SearchTerm filters by Name and ContactInfo
- GetStudentListQuery: returns only current tenant students
- GetStudentListQuery: pagination returns correct page and total
</details>
