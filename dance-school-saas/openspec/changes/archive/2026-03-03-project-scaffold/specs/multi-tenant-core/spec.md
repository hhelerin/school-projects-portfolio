## ADDED Requirements

### Requirement: Tenant Resolution Middleware

The system SHALL include a middleware that reads the tenant slug from the URL path position 1 and resolves the CompanyId from the database.

#### Scenario: Successful tenant resolution
- **WHEN** request path is /school-slug/page
- **THEN** middleware SHALL query Company by slug
- **AND** set ITenantContext.CompanyId and ITenantContext.Slug
- **AND** allow request to continue

#### Scenario: Tenant not found
- **WHEN** request path is /invalid-slug/page
- **THEN** middleware SHALL return 404 Not Found

### Requirement: Global Query Filters
The system SHALL apply global query filters to exclude IsDeleted = true and filter by CompanyId for ITenantEntity types.

#### Scenario: Automatic filtering
- **WHEN** any query is executed on ITenantEntity
- **THEN** WHERE CompanyId = currentTenant AND IsDeleted = false SHALL be applied automatically

### Requirement: Tenant Context Service
The system SHALL provide ITenantContext service scoped to the request lifetime.

#### Scenario: Service injection
- **WHEN** a service requests ITenantContext
- **THEN** the system SHALL provide the current tenant's CompanyId and Slug