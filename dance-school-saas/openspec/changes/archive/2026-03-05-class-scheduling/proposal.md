## Why

Dance schools need a robust scheduling system to manage recurring class templates and individual occurrences. 
Currently, there's no way to define recurring classes (e.g., "Beginner Ballet every Tuesday 18:00-19:00"), 
generate schedule instances, handle one-off changes like room swaps or cancellations, or view the weekly schedule. 
This change provides the foundation for all class operations before student enrollment and attendance tracking can be implemented.

## What Changes

- **Class template management**: Create, view, update, and delete recurring class templates with details like name, 
- instructor, dance style, level, capacity, and room
- **Recurrence pattern support**: Define weekly recurring schedules with day of week, start/end times, and date range
- **Schedule instance generation**: Automatically generate individual class occurrences from recurrence patterns
- **Schedule instance editing**: Edit single occurrences (room swap, time change) with conflict detection
- **Cancellation support**: Cancel individual class occurrences with reason tracking (visible, not deleted)
- **Weekly schedule view**: Filterable calendar view showing all classes for a given week
- **Conflict detection**: Prevent double-booking rooms by checking for time overlaps during class creation and editing

## Capabilities

### New Capabilities
- `class-management`: CRUD operations for class templates, including recurrence pattern definition
- `schedule-generation`: Generate individual class schedule instances from recurring patterns with conflict validation
- `schedule-instance-editing`: Edit or cancel single class occurrences with IsException tracking
- `schedule-view`: Weekly calendar view with filtering by style, level, room, and instructor

### Modified Capabilities
- None (this is a new feature area, no existing capability requirements are changing)

## Impact

- **New entities**: `Class`, `ClassSchedule`, `RecurrencePattern` (owned entity)
- **New controllers**: `ClassController`, `ClassScheduleController`
- **New views**: Classes (Index, Create, Detail, Edit), Schedule (Index, Edit)
- **New BLL features**: Commands and queries under `Features/Classes/` and `Features/ClassSchedules/`
- **New DTOs**: Class and schedule-related DTOs in `App.DTO/v1/ClassScheduling/`
- **Database**: EF Core migration `ClassScheduling` with indexes for tenant isolation and conflict queries
- **Navigation**: New "Operations" section in `_Layout.cshtml` with "Classes" and "Schedule" links
- **Authorization**: CompanyManager+ for create/edit/delete/cancel; any CompanyUser for view operations
