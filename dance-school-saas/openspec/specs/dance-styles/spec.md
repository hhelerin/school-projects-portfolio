## ADDED Requirements

### Requirement: Dance style CRUD operations
The system SHALL provide full CRUD operations for dance styles scoped to the current tenant.

#### Scenario: List dance styles
- **WHEN** a CompanyUser navigates to the dance styles index page
- **THEN** the system displays all active (non-deleted) dance styles for that tenant

#### Scenario: Create dance style
- **WHEN** a CompanyAdmin or CompanyManager submits a valid dance style creation form
- **THEN** the system creates the dance style with the current tenant's CompanyId

#### Scenario: Update dance style
- **WHEN** a CompanyAdmin or CompanyManager submits a valid dance style edit form
- **THEN** the system updates the dance style if it belongs to the current tenant

#### Scenario: Delete dance style
- **WHEN** a CompanyAdmin or CompanyManager requests deletion of a dance style
- **THEN** the system performs a soft delete (sets IsDeleted = true) if the style belongs to the current tenant

#### Scenario: Tenant isolation
- **WHEN** a CompanyUser views dance styles
- **THEN** the system only returns styles where CompanyId matches the current tenant

#### Scenario: Cross-tenant access denied
- **WHEN** a user attempts to edit or delete a dance style belonging to another tenant
- **THEN** the system returns a not found or access denied error
