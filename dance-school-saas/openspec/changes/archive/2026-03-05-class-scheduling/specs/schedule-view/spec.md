## ADDED Requirements

### Requirement: Weekly Schedule View
An authenticated CompanyUser SHALL be able to view a weekly schedule showing all class occurrences for a selected week.

#### Scenario: Default to current week
- **WHEN** a user navigates to the schedule page without specifying a week
- **THEN** the system displays the current week's schedule

#### Scenario: Navigate to specific week
- **WHEN** a user clicks previous/next week navigation or provides weekStart query parameter
- **THEN** the system displays the schedule for that specific week

#### Scenario: Schedule shows all required information
- **WHEN** a user views the weekly schedule
- **THEN** each schedule entry SHALL display: ClassName, DanceStyle, Level, Instructor, Room, StartTime-EndTime

### Requirement: Schedule Filtering
A CompanyUser SHALL be able to filter the weekly schedule by dance style, level, room, and instructor.

#### Scenario: Filter by dance style
- **WHEN** a user selects a DanceStyle from the filter dropdown
- **THEN** the schedule displays only classes with that DanceStyleId

#### Scenario: Filter by level
- **WHEN** a user selects a Level from the filter dropdown
- **THEN** the schedule displays only classes with that LevelId

#### Scenario: Filter by room
- **WHEN** a user selects a StudioRoom from the filter dropdown
- **THEN** the schedule displays only classes in that StudioRoomId

#### Scenario: Filter by instructor
- **WHEN** a user selects an Instructor from the filter dropdown
- **THEN** the schedule displays only classes with that InstructorId

#### Scenario: Multiple filters combined
- **WHEN** a user selects multiple filters
- **THEN** the schedule displays only classes matching ALL selected filters

### Requirement: Cancelled Instance Display
The system SHALL display cancelled schedule instances in the weekly view with visual indication.

#### Scenario: Cancelled class shown with strikethrough
- **WHEN** a schedule instance has IsCancelled = true
- **THEN** the system displays it with strikethrough formatting AND a cancelled badge

#### Scenario: Cancellation reason available
- **WHEN** a user views a cancelled schedule instance
- **THEN** the system displays the CancellationReason (if any)

### Requirement: Edited Instance Display
The system SHALL display manually edited schedule instances with visual indication.

#### Scenario: Edited class shows exception badge
- **WHEN** a schedule instance has IsException = true
- **THEN** the system displays an "edited" badge to indicate this was manually modified
