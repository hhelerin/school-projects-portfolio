## ADDED Requirements

### Requirement: Edit Single Schedule Instance
A CompanyManager or above SHALL be able to edit a single class schedule occurrence (date, start time, end time, room, details).

#### Scenario: Edit schedule instance successfully
- **WHEN** a CompanyManager edits a schedule instance with valid new values
- **THEN** the system updates the ClassSchedule record and sets IsException = true

#### Scenario: Edit with conflict detection
- **WHEN** a CompanyManager edits a schedule instance to a new room/time that conflicts with another class
- **THEN** the system returns a failure with conflict details and does not update the record

#### Scenario: Original values preserved as reference
- **WHEN** a CompanyManager views the edit form for a schedule instance
- **THEN** the system displays the original values for reference alongside the editable fields

### Requirement: Cancel Schedule Instance
A CompanyManager or above SHALL be able to cancel a single class schedule occurrence with a reason.

#### Scenario: Cancel schedule instance
- **WHEN** a CompanyManager cancels a schedule instance with a CancellationReason
- **THEN** the system sets IsCancelled = true and stores the CancellationReason

#### Scenario: Cancelled instance remains visible
- **WHEN** a schedule instance has been cancelled
- **THEN** the system still displays the cancelled instance in schedule views (marked as cancelled), rather than soft-deleting it

### Requirement: Exception Tracking
The system SHALL track which schedule instances were manually edited versus auto-generated from the recurrence pattern.

#### Scenario: Edited instance marked as exception
- **WHEN** any field of a schedule instance is edited (date, time, room, details)
- **THEN** the system sets IsException = true to indicate this is a manual exception

#### Scenario: View exception indicator
- **WHEN** a user views the schedule or class detail page
- **THEN** any instances with IsException = true are displayed with an "edited" indicator/badge
