## ADDED Requirements

### Requirement: Identity User Extension
The system SHALL extend IdentityUser with additional properties for the dance school domain.

#### Scenario: Extended user properties
- **WHEN** a user is created
- **THEN** AppUser SHALL have FirstName, LastName, nullable PreferredCompanyId

### Requirement: Company User Relationship
The system SHALL maintain relationships between users and companies.

#### Scenario: User company membership
- **WHEN** a user joins a company
- **THEN** CompanyUser record SHALL be created with AppUserId, CompanyId, IsActive, JoinedAt
- **THEN** AppUser record property PreferredCompanyId will be set to the newly joined company


### Requirement: Role-Based Authorization
The system SHALL support system and company-specific roles.

#### Scenario: System roles
- **WHEN** system administration is needed
- **THEN** roles like SystemAdmin, SystemSupport SHALL be available

#### Scenario: Company roles
- **WHEN** company-specific permissions are needed
- **THEN** roles like CompanyOwner, CompanyAdmin, CompanyEmployee SHALL be available