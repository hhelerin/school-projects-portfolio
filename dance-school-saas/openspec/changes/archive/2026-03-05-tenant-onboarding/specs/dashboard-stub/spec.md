## ADDED Requirements

### Requirement: School owner sees dashboard after registration
The system SHALL provide a basic dashboard page as the landing spot for school owners after successful registration.

#### Scenario: Display dashboard after registration
- **WHEN** a school owner successfully completes registration
- **THEN** the system redirects to `/{slug}/dashboard`
- **AND** the dashboard displays a welcome message with the owner's first name and school name

#### Scenario: Dashboard requires authentication
- **WHEN** an unauthenticated user navigates to `/{slug}/dashboard`
- **THEN** the system redirects to the login page

#### Scenario: Dashboard requires company membership
- **WHEN** an authenticated user who is not a member of the company navigates to `/{slug}/dashboard`
- **THEN** the system returns a 403 Forbidden response

#### Scenario: Dashboard content
- **WHEN** an authenticated CompanyUser navigates to `/{slug}/dashboard`
- **THEN** the dashboard displays:
  - Welcome message: "Welcome, {FirstName}. Your school {SchoolName} is set up."
  - Placeholder navigation menu with links for future features (Students, Classes, Instructors, etc.)

#### Scenario: Dashboard tenant resolution
- **WHEN** any authenticated user navigates to `/{slug}/dashboard`
- **THEN** the system resolves the tenant from the URL slug
- **AND** all data displayed is scoped to that company
