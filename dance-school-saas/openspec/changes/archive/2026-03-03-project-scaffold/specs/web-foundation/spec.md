## ADDED Requirements

### Requirement: MVC Controllers and Views
The system SHALL provide basic MVC structure with HomeController and views.

#### Scenario: Home page
- **WHEN** user navigates to the root
- **THEN** HomeController.Index SHALL return a view
- **AND** _Layout.cshtml SHALL include Bootstrap navbar

### Requirement: Dependency Injection Setup
The system SHALL configure DI for all services in Program.cs.

#### Scenario: Service registration
- **WHEN** the application starts
- **THEN** MediatR, FluentValidation, EF Core SHALL be registered
- **AND** TenantContext and CurrentUserService SHALL be scoped

### Requirement: Error Handling
The system SHALL provide global exception handling.

#### Scenario: Unhandled exception
- **WHEN** an unhandled exception occurs
- **THEN** Views/Shared/Error.cshtml SHALL be displayed