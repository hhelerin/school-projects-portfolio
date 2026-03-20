## ADDED Requirements

### Requirement: Instructor CRUD operations
The system SHALL provide full CRUD operations for instructor records scoped to the current tenant.

#### Scenario: List instructors
- **WHEN** a CompanyUser navigates to the instructors index page
- **THEN** the system displays all active (non-deleted) instructors for that tenant

#### Scenario: Create instructor
- **WHEN** a CompanyAdmin or CompanyManager submits a valid instructor creation form
- **THEN** the system creates the instructor with the current tenant's CompanyId

#### Scenario: Create instructor with user linkage
- **WHEN** a CompanyAdmin or CompanyManager creates an instructor and selects an existing CompanyUser
- **THEN** the system creates the instructor with AppUserId set to the selected user

#### Scenario: Create instructor without user
- **WHEN** a CompanyAdmin or CompanyManager creates an instructor without selecting a user
- **THEN** the system creates the instructor with null AppUserId

#### Scenario: Update instructor
- **WHEN** a CompanyAdmin or CompanyManager submits a valid instructor edit form
- **THEN** the system updates the instructor if it belongs to the current tenant

#### Scenario: Delete instructor
- **WHEN** a CompanyAdmin or CompanyManager requests deletion of an instructor
- **THEN** the system performs a soft delete (sets IsDeleted = true) if the instructor belongs to the current tenant

#### Scenario: Tenant isolation
- **WHEN** a CompanyUser views instructors
- **THEN** the system only returns instructors where CompanyId matches the current tenant

#### Scenario: Cross-tenant access denied
- **WHEN** a user attempts to edit or delete an instructor belonging to another tenant
- **THEN** the system returns a not found or access denied error

#### Scenario: AppUser dropdown population
- **WHEN** a CompanyAdmin or CompanyManager opens the instructor creation/edit form
- **THEN** the system presents a dropdown of CompanyUsers for the current tenant
