## ADDED Requirements

### Requirement: System roles are assigned to users
The system SHALL support system-level roles (SystemAdmin, SystemSupport, SystemBilling) that provide global access across all companies.

#### Scenario: SystemAdmin role assignment
- **WHEN** a user is assigned SystemAdmin role
- **THEN** the user can access all companies and perform administrative functions
- **AND** the role is stored in the system role table

#### Scenario: SystemSupport role assignment
- **WHEN** a user is assigned SystemSupport role
- **THEN** the user can access support functions across all companies
- **AND** the role is stored in the system role table

### Requirement: Company roles are assigned per company
The system SHALL support company-specific roles (CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee) that are assigned per company.

#### Scenario: CompanyOwner role assignment
- **WHEN** a user is assigned CompanyOwner role for a specific company
- **THEN** the user has full administrative access to that company
- **AND** the role assignment is stored in CompanyUserRole table
- **AND** the user is linked to the company via CompanyUser table

#### Scenario: CompanyAdmin role assignment
- **WHEN** a user is assigned CompanyAdmin role for a specific company
- **THEN** the user has administrative access to that company
- **AND** the role assignment is stored in CompanyUserRole table

#### Scenario: CompanyManager role assignment
- **WHEN** a user is assigned CompanyManager role for a specific company
- **THEN** the user has management access to that company
- **AND** the role assignment is stored in CompanyUserRole table

#### Scenario: CompanyEmployee role assignment
- **WHEN** a user is assigned CompanyEmployee role for a specific company
- **THEN** the user has basic employee access to that company
- **AND** the role assignment is stored in CompanyUserRole table

### Requirement: Users can belong to multiple companies
The system SHALL allow users to be associated with multiple companies with different roles in each company.

#### Scenario: User joins multiple companies
- **WHEN** a user is invited to join a second company
- **THEN** a new CompanyUser record is created for the second company
- **AND** the user can have different roles in each company
- **AND** the user's PreferredCompanyId can be set to default to one company

#### Scenario: User switches between companies
- **WHEN** an authenticated user navigates to a different company URL
- **THEN** the system validates the user has access to that company
- **AND** the tenant context is updated to the new company
- **AND** the user's role for that company is applied

### Requirement: Role-based authorization controls access
The system SHALL enforce role-based access control using [Authorize] attributes and role checks.

#### Scenario: SystemAdmin access to all companies
- **WHEN** a SystemAdmin user accesses any company
- **THEN** the user bypasses company-specific authorization checks
- **AND** the user has access to all system functions

#### Scenario: Company role access within company
- **WHEN** a user with CompanyAdmin role accesses their company
- **THEN** the user has access to company administrative functions
- **AND** the user is denied access to other companies

#### Scenario: Insufficient role access denied
- **WHEN** a user with CompanyEmployee role attempts to access CompanyAdmin functions
- **THEN** the system denies access with appropriate error message
- **AND** the user is redirected to an access denied page

### Requirement: Company user management
The system SHALL provide functionality to manage user associations with companies.

#### Scenario: Add user to company
- **WHEN** an authorized user adds another user to a company
- **THEN** a CompanyUser record is created with IsActive = true
- **AND** JoinedAt timestamp is set to current time
- **AND** default role is assigned (CompanyEmployee)

#### Scenario: Remove user from company
- **WHEN** an authorized user removes another user from a company
- **THEN** the CompanyUser record is marked as inactive
- **AND** all CompanyUserRole records for that user-company combination are removed

#### Scenario: Change user role within company
- **WHEN** an authorized user changes another user's role within a company
- **THEN** the CompanyUserRole record is updated with the new role
- **AND** the change is logged with timestamp

### Requirement: Role hierarchy and permissions
The system SHALL enforce role hierarchy where higher roles include permissions of lower roles.

#### Scenario: CompanyOwner has all permissions
- **WHEN** a CompanyOwner accesses any company function
- **THEN** the user has access to all company-level permissions
- **AND** CompanyOwner > CompanyAdmin > CompanyManager > CompanyEmployee

#### Scenario: Role permission inheritance
- **WHEN** a CompanyManager attempts to access CompanyEmployee functions
- **THEN** the system grants access (higher role includes lower permissions)
- **AND** the system denies access to CompanyAdmin functions (lower role cannot access higher)
