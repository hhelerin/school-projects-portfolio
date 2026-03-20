## 1. Domain Entities

- [ ] 1.1 Create Class entity in App.Domain with Id, CompanyId, Name, Details, StudioRoomId, InstructorId, DanceStyleId, LevelId, Capacity, IsDeleted
- [ ] 1.2 Create RecurrencePattern as owned entity on Class (DayOfWeek, StartTime, EndTime, RecurrenceStartDate, RecurrenceEndDate)
- [ ] 1.3 Create ClassSchedule entity in App.Domain with Id, CompanyId, ClassId, Date, StartTime, EndTime, StudioRoomId, IsCancelled, CancellationReason, IsException, Details, IsDeleted

## 2. EF Core Configuration

- [ ] 2.1 Add ValueConverter for DateOnly and TimeOnly in AppDbContext
- [ ] 2.2 Create IEntityTypeConfiguration<Class> with indexes on CompanyId and StudioRoomId
- [ ] 2.3 Create IEntityTypeConfiguration<ClassSchedule> with indexes on CompanyId, ClassId, and (StudioRoomId, Date)
- [ ] 2.4 Configure RecurrencePattern as owned entity on Class
- [ ] 2.5 Configure FK relationships with Restrict delete behavior
- [ ] 2.6 Create EF Core migration "ClassScheduling"

## 3. DTOs

- [ ] 3.1 Create ClassListItemDto (Id, Name, DanceStyleName, LevelName, InstructorName, StudioRoomName, Capacity, RecurrenceSummary)
- [ ] 3.2 Create ClassDetailDto (all ClassListItemDto fields plus Details, RecurrencePattern fields, upcoming schedule count)
- [ ] 3.3 Create ClassScheduleListItemDto (Id, ClassId, ClassName, Date, StartTime, EndTime, StudioRoomName, InstructorName, DanceStyleName, LevelName, IsCancelled, IsException, CancellationReason)
- [ ] 3.4 Create CreateClassInputModel for view model binding
- [ ] 3.5 Create EditClassScheduleInstanceInputModel
- [ ] 3.6 Create CancelClassScheduleInstanceInputModel

## 4. Class BLL - Commands and Queries

- [ ] 4.1 Implement CreateClassCommand with validation, conflict detection, and schedule generation
- [ ] 4.2 Implement UpdateClassCommand (name, details, capacity only)
- [ ] 4.3 Implement DeleteClassCommand (soft delete class + future schedule occurrences)
- [ ] 4.4 Implement GetClassByIdQuery returning ClassDetailDto
- [ ] 4.5 Implement GetClassListQuery returning list of ClassListItemDto
- [ ] 4.6 Create FluentValidation validators for all Class commands

## 5. ClassSchedule BLL - Commands and Queries

- [ ] 5.1 Implement EditClassScheduleInstanceCommand with conflict detection and IsException flag
- [ ] 5.2 Implement CancelClassScheduleInstanceCommand
- [ ] 5.3 Implement GetScheduleByWeekQuery with optional filters (DanceStyleId, LevelId, StudioRoomId, InstructorId)
- [ ] 5.4 Implement GetClassSchedulesByClassQuery returning future occurrences
- [ ] 5.5 Create private conflict check method for reuse
- [ ] 5.6 Create FluentValidation validators for ClassSchedule commands

## 6. Controllers

- [ ] 6.1 Create ClassController with CRUD endpoints (/{slug}/classes/)
- [ ] 6.2 Create ClassScheduleController with edit/cancel endpoints (/{slug}/schedule/)
- [ ] 6.3 Add authorization attributes (CompanyManager+ for create/edit/delete/cancel)
- [ ] 6.4 Implement conflict error handling in controllers

## 7. Views - Classes

- [ ] 7.1 Create Classes/Index.cshtml with Bootstrap table and CRUD buttons
- [ ] 7.2 Create Classes/Create.cshtml with form and dropdowns
- [ ] 7.3 Create Classes/Detail.cshtml with class info and schedule list
- [ ] 7.4 Create Classes/Edit.cshtml (name, details, capacity only)
- [ ] 7.5 Add conflict error display in create/edit views

## 8. Views - Schedule

- [ ] 8.1 Create Schedule/Index.cshtml with weekly calendar/table view
- [ ] 8.2 Add filter bar (week navigation, style/level/room/instructor dropdowns)
- [ ] 8.3 Display cancelled entries with strikethrough + badge
- [ ] 8.4 Display exception entries with badge
- [ ] 8.5 Create Schedule/Edit.cshtml for single occurrence editing

## 9. Navigation

- [ ] 9.1 Add "Operations" section to _Layout.cshtml navigation
- [ ] 9.2 Add "Classes" link under Operations
- [ ] 9.3 Add "Schedule" link under Operations

## 10. Tests

- [ ] 10.1 Write CreateClassCommand test: generates correct number of occurrences for 4-week recurrence
- [ ] 10.2 Write CreateClassCommand test: returns failure when conflict exists, does not save
- [ ] 10.3 Write CreateClassCommand test: saves class and all occurrences in one transaction when no conflicts
- [ ] 10.4 Write EditClassScheduleInstanceCommand test: sets IsException = true
- [ ] 10.5 Write EditClassScheduleInstanceCommand test: returns failure on conflict
- [ ] 10.6 Write CancelClassScheduleInstanceCommand test: sets IsCancelled, does not soft delete
- [ ] 10.7 Write DeleteClassCommand test: soft deletes future occurrences, leaves past untouched
- [ ] 10.8 Write GetScheduleByWeekQuery test: returns only occurrences for requested week, respects tenant isolation
