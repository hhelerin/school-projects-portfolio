## ADDED Requirements

### Requirement: Register a new student
The system SHALL allow staff to register a new student with name and optional contact information, personal ID, and details.

#### Scenario: Successful student registration
- **WHEN** staff submits valid registration data (Name is required, other fields optional)
- **THEN** system creates a new Student record with CompanyId set from ITenantContext and returns the new StudentId

#### Scenario: Registration with linked AppUser
- **WHEN** staff provides an AppUserId to link to the student
- **THEN** system SHALL validate that the AppUser exists and is not already linked to another Student in the same tenant

#### Scenario: Duplicate detection warning
- **WHEN** a Student with the same Name AND ContactInfo already exists in this tenant
- **THEN** system SHALL return a duplicate warning (not hard-block) allowing staff to confirm or override via ConfirmDuplicate flag

#### Scenario: ConfirmDuplicate bypasses check
- **WHEN** staff submits with ConfirmDuplicate = true
- **THEN** system SHALL bypass the duplicate check and save the student regardless

### Requirement: Update student information
The system SHALL allow staff to update existing student information.

#### Scenario: Successful update
- **WHEN** staff submits valid update data for an existing student
- **THEN** system SHALL update the Student record and return success

#### Scenario: Update with AppUser change
- **WHEN** staff changes the AppUserId to a different user
- **THEN** system SHALL validate the new AppUser exists and is not already linked to another Student in the same tenant

#### Scenario: Update non-existent student
- **WHEN** staff attempts to update a student that does not belong to the current tenant
- **THEN** system SHALL return a failure result

### Requirement: Delete a student
The system SHALL allow staff to soft-delete (mark as deleted) student records.

#### Scenario: Successful soft delete
- **WHEN** staff requests to delete a student with no active packages
- **THEN** system SHALL set IsDeleted = true and return success

#### Scenario: Delete blocked by active packages
- **WHEN** staff requests to delete a student with any active (non-expired, non-deleted) Package records
- **THEN** system SHALL return failure with message "Student has active packages. Deactivate packages before removing the student."

### Requirement: List students with search and pagination
The system SHALL allow staff to search and paginate through the student list.

#### Scenario: List all students
- **WHEN** staff views the student list without a search term
- **THEN** system SHALL return a paged list of all non-deleted students for the current tenant

#### Scenario: Search by name or contact
- **WHEN** staff provides a SearchTerm
- **THEN** system SHALL filter students where Name OR ContactInfo contains the search term

#### Scenario: Pagination returns correct page
- **WHEN** staff requests a specific page number and page size
- **THEN** system SHALL return only the requested page of results with correct pagination metadata

### Requirement: View student details
The system SHALL allow staff to view detailed information about a student.

#### Scenario: View student profile
- **WHEN** staff requests to view a student's detail page
- **THEN** system SHALL return the student's Name, PersonalId, ContactInfo, Details, linked AppUser (if any), and metadata (CreatedAt, UpdatedAt)

#### Scenario: View non-existent student
- **WHEN** staff requests to view a student that does not belong to the current tenant
- **THEN** system SHALL return a failure result

### Requirement: Placeholder sections for future features
The student detail view SHALL display placeholder sections for features not yet implemented.

#### Scenario: View placeholder packages section
- **WHEN** staff views a student's detail page
- **THEN** system SHALL display an "Active Packages" card with "Coming soon" placeholder text (populated in Change 7)

#### Scenario: View placeholder trial records section
- **WHEN** staff views a student's detail page
- **THEN** system SHALL display a "Trial Records" card with "Coming soon" placeholder text (populated in Change 8)

#### Scenario: View placeholder attendance section
- **WHEN** staff views a student's detail page
- **THEN** system SHALL display an "Attendance" card with "Coming soon" placeholder text (populated in Change 8)

#### Scenario: View placeholder showcase eligibility section
- **WHEN** staff views a student's detail page
- **THEN** system SHALL display a "Showcase Eligibility" card with "Coming soon" placeholder text (populated in Change 9)

### Requirement: Authorization for student operations
The system SHALL enforce role-based authorization for student management operations.

#### Scenario: Read access for all company users
- **WHEN** an authenticated CompanyEmployee, CompanyManager, CompanyAdmin, or CompanyOwner requests GET / or GET /{id}
- **THEN** system SHALL allow access

#### Scenario: Write access for employees and above
- **WHEN** an authenticated CompanyEmployee, CompanyManager, CompanyAdmin, or CompanyOwner requests GET /create, POST /create, GET /{id}/edit, or POST /{id}/edit
- **THEN** system SHALL allow access

#### Scenario: Delete access restricted to admins
- **WHEN** a CompanyEmployee or CompanyManager requests POST /{id}/delete
- **THEN** system SHALL deny access

#### Scenario: Delete access for admins and owners
- **WHEN** an authenticated CompanyAdmin or CompanyOwner requests POST /{id}/delete
- **THEN** system SHALL allow access
