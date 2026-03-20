## Context

This change implements class scheduling for the dance school platform. The system needs to manage recurring class templates 
(e.g., "Beginner Ballet every Tuesday") and individual schedule occurrences. 
Dance school staff need to create templates, generate recurring schedules, handle room swaps/cancellations, and view the weekly schedule.

**Current State**: The platform has basic school configuration (studios, rooms, dance styles, levels, instructors) but no class scheduling capabilities.

**Constraints**:
- All data must be tenant-isolated via CompanyId
- Controllers scoped under /{slug}/ for multi-tenancy
- Must follow established controller/view/handler patterns from existing changes
- Soft deletes only (IsDeleted flag)
- TimeOnly and DateOnly require SQLite ValueConverters
- Conflict detection needed for room bookings

## Goals / Non-Goals

**Goals:**
- Enable dance schools to create and manage recurring class templates
- Generate individual schedule occurrences from recurrence patterns
- Support per-occurrence edits (room swaps, time changes) with conflict detection
- Provide cancellation functionality with reason tracking
- Display weekly schedule view with filtering capabilities
- Prevent double-booking of rooms through conflict detection

**Non-Goals:**
- Student enrollment or check-in (Change 8)
- Attendance tracking (Change 8)
- Waitlists
- Email notifications for cancellations
- Capacity enforcement at check-in time (Change 8)

## Decisions

### 1. Recurrence Pattern as Owned Entity
**Decision**: Store RecurrencePattern as an owned entity on Class rather than a separate table.

**Rationale**: RecurrencePattern only exists in the context of a Class template and has no independent lifecycle. 
Using an owned entity simplifies CRUD operations and ensures consistency.

**Alternative Considered**: Separate table with foreign key — rejected because recurrence patterns are never queried independently of their class.

### 2. Soft Delete for Class + Future Schedules
**Decision**: When deleting a Class, soft-delete the class AND all future (Date >= today) schedule occurrences that aren't already cancelled.

**Rationale**: Past occurrences must remain for attendance history. Cancelled occurrences remain visible to show what was cancelled. 
Future occurrences are effectively "cancelled" by the class deletion.

### 3. IsException Flag for Edited Instances
**Decision**: Mark edited schedule instances with IsException = true to distinguish them from generated occurrences.

**Rationale**: This allows users to see which schedule instances were manually modified (room swap, time change) versus the auto-generated default. 
Useful for debugging and reporting.

### 4. Conflict Detection at Create/Edit Time
**Decision**: Run conflict check during CreateClassCommand and EditClassScheduleInstanceCommand, returning failure if conflicts exist.

**Rationale**: Prevents double-booking rooms. The conflict check compares: same StudioRoomId + same Date + overlapping time ranges.

**Alternative Considered**: Lazy conflict resolution — rejected because partial saves could leave inconsistent state.

### 5. DateOnly/TimeOnly Storage
**Decision**: Use EF Core ValueConverter for DateOnly and TimeOnly to TEXT in SQLite.

**Rationale**: SQLite doesn't natively support these types. TEXT storage with conversion is the recommended approach.

## Risks / Trade-offs

- **[Risk] Recurrence pattern changes after generation**: If someone edits the recurrence pattern of an existing class, what happens to already-generated instances?
  - **Mitigation**: Out of scope for this change. Schedule changes must be done per-occurrence. Document this limitation.

- **[Risk] Large number of generated instances**: Generating occurrences far into the future could create thousands of records.
  - **Mitigation**: RecurrenceEndDate provides a natural limit. Staff can regenerate with new dates if needed.

- **[Risk] Concurrent edits**: Two managers could edit the same schedule instance simultaneously.
  - **Mitigation**: Not addressed in MVP. EF Core optimistic concurrency can be added later if needed.

- **[Risk] Time zone handling**: TimeOnly is time-zone agnostic, but DateOnly could have DST implications.
  - **Mitigation**: Keep it simple — classes are scheduled in local time as entered. No DST calculations.
