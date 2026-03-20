## ADDED Requirements

### Requirement: Class Template Creation
A CompanyManager or above SHALL be able to create a new class template with name, details, studio room, instructor, dance style, level, capacity, and recurrence pattern.

#### Scenario: Successful class creation
- **WHEN** a CompanyManager submits the create class form with valid data
- **THEN** the system creates the Class record with the owned RecurrencePattern and generates all ClassSchedule occurrences for the recurrence date range

#### Scenario: Invalid foreign key reference
- **WHEN** a CompanyManager submits the create class form with a non-existent StudioRoomId, InstructorId, DanceStyleId, or LevelId
- **THEN** the system returns a validation error and does not create the class

#### Scenario: Invalid recurrence date range
- **WHEN** a CompanyManager submits the create class form with RecurrenceEndDate <= RecurrenceStartDate
- **THEN** the system returns a validation error and does not create the class

#### Scenario: Room conflict at creation
- **WHEN** a CompanyManager submits the create class form that would create schedule occurrences overlapping with existing non-cancelled classes in the same room
- **THEN** the system returns a failure with conflict details and does not create any records

### Requirement: Class Template Viewing
An authenticated CompanyUser SHALL be able to view the list of all class templates for their company.

#### Scenario: View class list
- **WHEN** a CompanyUser navigates to the classes index page
- **THEN** the system displays a table with Name, DanceStyle, Level, Instructor, Room, Recurrence summary, and Capacity for each class

#### Scenario: View class details
- **WHEN** a CompanyUser clicks on a class to view its details
- **THEN** the system displays all class information including the recurrence pattern and a list of upcoming schedule occurrences

### Requirement: Class Template Update
A CompanyManager or above SHALL be able to update a class template's name, details, and capacity only.

#### Scenario: Successful class update
- **WHEN** a CompanyManager submits the edit class form with valid data
- **THEN** the system updates the Class record and returns success

#### Scenario: Schedule change attempted via edit
- **WHEN** a CompanyManager attempts to change the recurrence pattern or room via the class edit form
- **THEN** the system does not allow these changes (must use schedule instance editing)

### Requirement: Class Template Deletion
A CompanyManager or above SHALL be able to soft-delete a class template.

#### Scenario: Delete class with future schedules
- **WHEN** a CompanyManager deletes a class that has future schedule occurrences
- **THEN** the system soft-deletes the Class and soft-deletes all future (Date >= today) ClassSchedule occurrences that are not already cancelled

#### Scenario: Delete class preserves past schedules
- **WHEN** a CompanyManager deletes a class that has past schedule occurrences
- **THEN** the system preserves all past ClassSchedule occurrences (they remain for attendance history)
