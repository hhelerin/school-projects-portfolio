## ADDED Requirements

### Requirement: School owner can register a new school
The system SHALL provide a public registration page allowing dance school owners to create a new school account.

#### Scenario: Display registration form
- **WHEN** an unauthenticated user navigates to `/signup`
- **THEN** the system displays a registration form with fields for owner first name, last name, email, password, confirm password, school name, slug, registration code (optional), and VAT code (optional)

#### Scenario: Slug auto-generation from school name
- **WHEN** the user enters "My Dance School" in the School Name field
- **THEN** the system auto-generates "my-dance-school" in the Slug field
- **AND** the user can manually edit the slug

#### Scenario: Slug validation - unique check
- **WHEN** the user submits the form with a slug that already exists (case-insensitive)
- **THEN** the system returns the form with an error: "This school URL is already taken"

#### Scenario: Slug validation - format check
- **WHEN** the user submits the form with a slug containing special characters or spaces
- **THEN** the system returns the form with an error: "URL can only contain lowercase letters, numbers, and hyphens"

#### Scenario: Email validation - unique check
- **WHEN** the user submits the form with an email that already exists
- **THEN** the system asks for confirmation to connect the creatable dance school with the existing AppUser
- **THEN** the user is redirected to login page and after login they are redirected to the school sign-up page, which is pre-filled with the previously filled data
- **AND** upon positive confirmation, the system continues to register the school, using the existing AppUser

#### Scenario: Password validation
- **WHEN** the user submits the form with a password shorter than 8 characters or missing required complexity (uppercase, digit)
- **THEN** the system returns the form with password validation errors

#### Scenario: Successful school registration
- **WHEN** the user submits valid registration data
- **THEN** the system creates a Company record with IsActive=true and SubscriptionTier=Free
- **AND** the system creates an AppUser for the owner or uses the existing AppUser data
- **AND** the system creates a CompanyUser join record with IsActive=true and JoinedAt=current timestamp
- **AND** the system assigns the CompanyOwner role to the CompanyUser
- **AND** the system signs the user in automatically
- **AND** the system redirects to `/{slug}/dashboard`

#### Scenario: Failed registration - duplicate slug
- **WHEN** the user submits a registration with a slug that already exists
- **THEN** the system returns Result.Failure with error "A school with this URL already exists"
- **AND** the controller displays the form with the error message

#### Scenario: Transaction rollback on partial failure
- **WHEN** the school creation process fails after creating the Company but before completing all steps
- **THEN** the system rolls back all changes and no partial data persists
