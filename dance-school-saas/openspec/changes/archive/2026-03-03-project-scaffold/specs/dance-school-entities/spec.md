## ADDED Requirements

### Requirement: Base Entity Properties
All domain entities SHALL inherit from BaseEntity with standard audit properties.

#### Scenario: Entity inheritance
- **WHEN** a new entity class is created
- **THEN** it SHALL inherit from BaseEntity
- **AND** have Id (Guid), CreatedAt, UpdatedAt (DateTime), IsDeleted (bool)

### Requirement: Tenant Entity Extension
Entities requiring multi-tenancy SHALL implement ITenantEntity with CompanyId.

#### Scenario: Tenant-scoped entity
- **WHEN** an entity needs tenant isolation
- **THEN** it SHALL implement ITenantEntity
- **AND** have CompanyId (Guid) property

### Requirement: Core Business Entities
The system SHALL define Company and CompanySettings entities.

#### Scenario: Company entity
- **WHEN** the system needs to store company information
- **THEN** Company entity SHALL have Id, Name, Slug, IsActive, SubscriptionTier, etc.

#### Scenario: CompanySettings entity
- **WHEN** the system needs tenant-specific settings
- **THEN** CompanySettings entity SHALL have Id, CompanyId, Key, Value