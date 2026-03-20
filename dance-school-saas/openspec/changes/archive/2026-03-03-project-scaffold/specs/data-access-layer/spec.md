## ADDED Requirements

### Requirement: Repository Pattern
The system SHALL implement IRepository<T> and IUnitOfWork for data access.

#### Scenario: Generic repository
- **WHEN** data access is needed
- **THEN** IRepository<T> SHALL provide CRUD operations
- **AND** support soft delete filtering

### Requirement: EF Core Configuration
The system SHALL configure AppDbContext with Identity and global filters.

#### Scenario: Context setup
- **WHEN** the application starts
- **THEN** AppDbContext SHALL inherit IdentityDbContext<AppUser>
- **AND** apply global query filters for IsDeleted and CompanyId

### Requirement: Database Migrations
The system SHALL use EF Core migrations for schema changes.

#### Scenario: Migration execution
- **WHEN** database schema changes
- **THEN** dotnet ef migrations add SHALL create migration files
- **AND** dotnet ef database update SHALL apply changes