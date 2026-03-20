## ADDED Requirements

### Requirement: Level CRUD operations
The system SHALL provide full CRUD operations for proficiency levels scoped to the current tenant.

#### Scenario: List levels
- **WHEN** a CompanyUser navigates to the levels index page
- **THEN** the system displays all active (non-deleted) levels for that tenant

#### Scenario: Create level
- **WHEN** a CompanyAdmin or CompanyManager submits a valid level creation form
- **THEN** the system creates the level with the current tenant's CompanyId

#### Scenario: Update level
- **WHEN** a CompanyAdmin or CompanyManager submits a valid level edit form
- **THEN** the system updates the level if it belongs to the current tenant

#### Scenario: Delete level
- **WHEN** a CompanyAdmin or CompanyManager requests deletion of a level
- **THEN** the system performs a soft delete (sets IsDeleted = true) if the level belongs to the current tenant

#### Scenario: Tenant isolation
- **WHEN** a CompanyUser views levels
- **THEN** the system only returns levels where CompanyId matches the current tenant

#### Scenario: Cross-tenant access denied
- **WHEN** a user attempts to edit or delete a level belonging to another tenant
- **THEN** the system returns a not found or access denied error
