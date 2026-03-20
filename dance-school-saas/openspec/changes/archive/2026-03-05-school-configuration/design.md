## Context

The Dance School Platform currently has authentication, tenant onboarding, and basic identity management in place. 
To support class scheduling and student management (upcoming Change 5), the system first needs foundational configuration data: 
what dance styles are offered, what proficiency levels exist, where classes are held (studios and rooms), and who teaches them (instructors).

This design addresses the CRUD infrastructure for these configuration entities while maintaining strict tenant isolation 
and following the established CQRS + MVC pattern from previous changes.

## Goals / Non-Goals

**Goals:**
- Provide complete CRUD operations for DanceStyle, Level, Studio, StudioRoom, and Instructor entities
- Support nested resource management (Studios → Rooms → Features)
- Maintain tenant isolation: all tenant-scoped queries filter by CompanyId
- Implement soft deletes for all business entities
- Follow existing architectural patterns: MediatR handlers, FluentValidation, Result<T> returns
- Enable role-based authorization (CompanyAdmin/CompanyManager for writes)

**Non-Goals:**
- Class scheduling (Change 5)
- Student management or enrollment
- Style-level compatibility constraints (post-MVP)
- Instructor-style qualification tracking (post-MVP)
- Studio-feature compatibility enforcement (post-MVP)
- Billing or subscription integration

## Decisions

**1. Shared vs Tenant-Scoped Feature Table**
- Decision: Feature is a shared reference table (no CompanyId)
- Rationale: Room features (sprung floor, mirrors) are universal concepts. Each company doesn't need to define their own "mirrors" feature.
- Alternative considered: Making Feature tenant-scoped — rejected to avoid redundant data entry across tenants

**2. StudioRoom as Child of Studio**
- Decision: StudioRoom has a required StudioId FK; studios must exist before rooms
- Rationale: Rooms are always within a specific studio location. Nesting URLs (/studios/{id}/rooms/) reflect this relationship
- Constraint: Cannot delete a studio that has rooms (FK restrict delete)

**3. Instructor-AppUser as Optional Link**
- Decision: Instructor.AppUserId is nullable Guid
- Rationale: Instructors may exist in the system before being invited as users, or may never need portal access
- Future consideration: Later changes may enforce user account creation for active instructors

**4. Flat Feature Folder Structure**
- Decision: All handlers per entity go in `App.BLL/Features/{Entity}/`
- Rationale: Consistent with existing codebase pattern. Keeps related commands/queries colocated
- Example: `Features/DanceStyles/GetListDanceStyleQuery.cs`, `CreateDanceStyleCommand.cs`, etc.

**5. DTO Separation Pattern**
- Decision: Four DTO types per entity: ListItemDto, DetailDto, CreateInputModel, UpdateInputModel
- Rationale: List views need fewer fields than detail views; input models may have different validation needs than read models
- Precedent: Follows pattern established in previous changes

**6. Soft Delete Implementation**
- Decision: All tenant-scoped entities implement IsDeleted flag
- Rationale: Prevents accidental data loss; maintains referential integrity for historical records
- Implementation: EF Core global query filter excludes IsDeleted = true; DeleteCommand sets flag

## Risks / Trade-offs

**[Risk] Feature table changes affect all tenants**
→ Mitigation: Features are seed-only in this change; modifications require migration/deployment coordination

**[Risk] Studio deletion blocked by existing rooms**
→ Mitigation: UI will show room count and require room deletion/archival first; clear error messaging

**[Risk] Instructor without AppUser cannot log in**
→ Mitigation: UI indicates "No Portal Access" for instructors lacking user linkage; future change will add invitation flow

**[Trade-off] No hard deletes means data accumulates**
→ Accepted: SQLite storage is cheap; soft deletes support audit trails and accidental recovery

## Migration Plan

1. Generate EF Core migration: `dotnet ef migrations add SchoolConfiguration`
2. Migration includes:
   - All new entity tables
   - Indexes on CompanyId columns
   - FK relationships with restrict delete
   - Seed data for Feature table (6 common values)
3. Apply migration: `dotnet ef database update`
4. No rollback strategy needed — new tables are additive only

## Open Questions

*None — design is complete for this change scope*
