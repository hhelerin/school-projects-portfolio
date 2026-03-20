## Why

Dance schools need to configure their operational foundation before they can manage classes, students, and schedules. 
Without proper configuration of dance styles, proficiency levels, physical studios with rooms, and instructors, 
the platform cannot support core business workflows. 
This change delivers the foundational data management capabilities required for day-to-day dance school operations.

## What Changes

- Add six new domain entities for school configuration:
  - **DanceStyle** — tenant-scoped reference data for dance genres (Salsa, Ballet, etc.)
  - **Level** — tenant-scoped proficiency levels (Beginner, Intermediate, Advanced, etc.)
  - **Feature** — shared reference table for physical room amenities (sprung floor, mirrors, barres, etc.)
  - **Studio** — tenant-scoped physical location with contact info
  - **StudioRoom** — rooms within a studio, linkable to features
  - **StudioFeature** — join table linking rooms to features with optional validity period
  - **Instructor** — tenant-scoped staff records with optional AppUser linkage

- Build full CQRS command/query handlers for all tenant-scoped entities
- Create MVC controllers with CRUD views for DanceStyles, Levels, Studios, StudioRooms, and Instructors
- Add EF Core configurations with proper indexes, foreign keys, and seed data for shared Feature table
- Implement authorization policies: CompanyAdmin/CompanyManager/CompanyOwner for write operations, any CompanyUser for read
- Generate EF Core migration "SchoolConfiguration"
- Add navigation links under "Configuration" section in dashboard

## Capabilities

### New Capabilities
- `dance-styles`: CRUD management for dance style reference data
- `levels`: CRUD management for proficiency level reference data
- `studios`: Management of physical studio locations with room nesting
- `instructors`: Staff management with optional user account linkage

### Modified Capabilities
- *None* — this change introduces only new capabilities

## Impact

- **Database**: New tables for all entities; indexes on CompanyId columns; seed data for Feature table
- **Domain Layer**: Six new entity classes in App.Domain
- **Application Layer**: ~25 new CQRS handlers (queries and commands) with FluentValidation validators
- **Web Layer**: 5 new controllers with ~20 view files (Index, Create, Edit, Detail where applicable)
- **Authorization**: New role-based access control on configuration routes
- **UI**: New "Configuration" navigation section in dashboard layout
