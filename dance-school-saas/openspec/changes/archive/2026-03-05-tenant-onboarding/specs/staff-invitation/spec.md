## ADDED Requirements

### Requirement: CompanyAdmin can invite staff members
The system SHALL allow CompanyAdmin or CompanyOwner users to invite new staff members to their school by email.

#### Scenario: Display invitation form
- **WHEN** an authenticated CompanyAdmin navigates to `/{slug}/staff/invite`
- **THEN** the system displays an invitation form with fields for email, first name, last name, and a dropdown of company roles
- **AND** the role dropdown contains all CompanyRoles for the current tenant

#### Scenario: Authorization check - anonymous user
- **WHEN** an unauthenticated user navigates to `/{slug}/staff/invite`
- **THEN** the system redirects to the login page

#### Scenario: Authorization check - non-admin user
- **WHEN** an authenticated user without CompanyAdmin or CompanyOwner role navigates to `/{slug}/staff/invite`
- **THEN** the system returns a 403 Forbidden response

#### Scenario: Invite existing platform user
- **WHEN** the admin submits the invitation form with an email that already exists in the system
- **THEN** the system creates a CompanyUser record linking the existing AppUser to the current company with IsActive=true
- **AND** the system creates a CompanyUserRole record assigning the selected role
- **AND** the system redirects to `/{slug}/staff` with a success message

#### Scenario: Invite new user
- **WHEN** the admin submits the invitation form with an email that does not exist in the system
- **THEN** the system creates an AppUser with the provided email, first name, last name, and a temporary random password
- **AND** the system creates a CompanyUser record with IsActive=true
- **AND** the system creates a CompanyUserRole record assigning the selected role
- **AND** the system logs the temporary password to the console
- **AND** the system redirects to `/{slug}/staff` with a success message

#### Scenario: Validation - missing required fields
- **WHEN** the admin submits the form with missing email, first name, or last name
- **THEN** the system returns the form with validation errors for the missing fields

#### Scenario: Validation - invalid email format
- **WHEN** the admin submits the form with an invalid email format
- **THEN** the system returns the form with a validation error: "Please enter a valid email address"

#### Scenario: Validation - user already member of company
- **WHEN** the admin submits the form with an email belonging to a user who is already a member of this company
- **THEN** the system returns the form with an error: "This user is already a member of your school"
- **AND** the system offers to assign another CompanyRole to the AppUser

#### Scenario: Role dropdown population
- **WHEN** the invitation form is displayed
- **THEN** the role dropdown contains only CompanyRoles (not SystemRoles)
- **AND** the dropdown is sorted alphabetically by role name
- **AND** the dropdown is sorted alphabetically by role name
