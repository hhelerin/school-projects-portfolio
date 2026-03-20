## ADDED Requirements

### Requirement: Recurrence Pattern Definition
The system SHALL support defining a recurrence pattern for class templates with DayOfWeek, StartTime, EndTime, RecurrenceStartDate, and RecurrenceEndDate.

#### Scenario: Weekly recurrence pattern
- **WHEN** a class template is created with a recurrence pattern specifying DayOfWeek = Tuesday, StartTime = 18:00, EndTime = 19:00, StartDate = 2026-01-01, EndDate = 2026-12-31
- **THEN** the system generates ClassSchedule occurrences for every Tuesday between the start and end dates

#### Scenario: Four-week recurrence generates correct count
- **WHEN** a class template is created with a recurrence pattern for a single day of the week spanning exactly 4 weeks
- **THEN** the system generates exactly 4 ClassSchedule occurrences

### Requirement: Schedule Instance Generation
When a class template is created, the system SHALL automatically generate all ClassSchedule occurrences for the recurrence date range.

#### Scenario: Generate all occurrences at class creation
- **WHEN** a valid class template with recurrence pattern is created
- **THEN** the system generates ClassSchedule records for each matching day in the range and saves them in the same transaction as the Class

#### Scenario: No occurrences on conflict
- **WHEN** generating schedule occurrences would create a conflict with existing non-cancelled classes in the same room
- **THEN** the system returns a failure with conflict details and does not save any records (neither the Class nor any ClassSchedule)

### Requirement: Conflict Detection
The system SHALL detect scheduling conflicts when a new class would use the same room at overlapping times.

#### Scenario: Conflict with existing class
- **WHEN** a new class would be scheduled in StudioRoom A on 2026-03-10 from 18:00-19:00 and StudioRoom A already has a non-cancelled class on that date from 18:30-19:30
- **THEN** the system detects this as a conflict (time ranges overlap: new StartTime < existing EndTime AND new EndTime > existing StartTime)

#### Scenario: No conflict with different room
- **WHEN** a new class would be scheduled in StudioRoom A on 2026-03-10 from 18:00-19:00 and StudioRoom B already has a class at that time
- **THEN** the system does not detect a conflict (different StudioRoomId)

#### Scenario: No conflict with cancelled class
- **WHEN** a new class would be scheduled in a room at a time where an existing class is marked as IsCancelled = true
- **THEN** the system does not detect a conflict (cancelled classes are not considered)

#### Scenario: No conflict with deleted class
- **WHEN** a new class would be scheduled in a room at a time where an existing class is marked as IsDeleted = true
- **THEN** the system does not detect a conflict (deleted classes are not considered)
