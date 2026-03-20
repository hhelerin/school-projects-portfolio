## ADDED Requirements

### Requirement: User can register with email, firstname, lastname and password
The system SHALL allow new users to register using their email, firstname, lastname, address and a secure password. Registration creates a new user account in the system.

#### Scenario: Successful registration
- **WHEN** user submits registration form with valid email, firstname, lastname, password, and confirm password
- **THEN** system creates a new user account
- **AND** system displays success message
- **AND** user can log in with credentials

#### Scenario: Registration with invalid email
- **WHEN** user submits registration form with invalid email format
- **THEN** system displays validation error for email field
- **AND** user account is not created

#### Scenario: Registration with weak password
- **WHEN** user submits registration form with password not meeting complexity requirements
- **THEN** system displays validation error describing password requirements
- **AND** user account is not created

#### Scenario: Registration with mismatched passwords
- **WHEN** user submits registration form where password and confirm password do not match
- **THEN** system displays validation error
- **AND** user account is not created

#### Scenario: Registration with duplicate email
- **WHEN** user submits registration form with an email that already exists
- **THEN** system displays error message indicating email is already registered
- **AND** user account is not created

### Requirement: User can log in with email and password
The system SHALL authenticate registered users using their email and password. Successful login creates an authenticated session.

#### Scenario: Successful login
- **WHEN** user enters correct email and password
- **THEN** system authenticates the user
- **AND** system redirects to home page or intended destination
- **AND** user interface shows logged-in state

#### Scenario: Login with incorrect password
- **WHEN** user enters correct email but incorrect password
- **THEN** system displays error message
- **AND** user remains on login page
- **AND** user is not authenticated

#### Scenario: Login with non-existent email
- **WHEN** user enters an email that does not exist in the system
- **THEN** system displays error message
- **AND** user remains on login page
- **AND** user is not authenticated

#### Scenario: Login with locked account
- **WHEN** user attempts to login to an account that has been locked
- **THEN** system displays error message about account being locked
- **AND** user is not authenticated

### Requirement: User can log out
The system SHALL allow authenticated users to end their session by logging out.

#### Scenario: Successful logout
- **WHEN** authenticated user clicks logout button
- **THEN** system ends the user session
- **AND** user is redirected to login page or home page
- **AND** user interface shows logged-out state

### Requirement: User can reset forgotten password
The system SHALL allow users to reset their password through an email-based reset flow when they have forgotten their password.

#### Scenario: Request password reset
- **WHEN** user enters their email on the password reset request page
- **THEN** system validates the email exists
- **AND** system sends password reset instructions to the email
- **AND** system displays confirmation message

#### Scenario: Request password reset for non-existent email
- **WHEN** user enters an email that does not exist in the system
- **THEN** system displays a generic message (does not reveal if email exists for security)

#### Scenario: Reset password with valid token
- **WHEN** user clicks password reset link from email and enters new password
- **THEN** system validates the reset token is valid and not expired
- **AND** system updates the user's password
- **AND** user can log in with new password

#### Scenario: Reset password with expired token
- **WHEN** user clicks password reset link that has expired
- **THEN** system displays error message about expired link
- **AND** user must request a new reset email

### Requirement: User can change password while logged in
The system SHALL allow authenticated users to change their password by providing their current password and a new password.

#### Scenario: Successful password change
- **WHEN** authenticated user enters correct current password and new valid password
- **THEN** system updates the password
- **AND** system displays success message
- **AND** user remains logged in

#### Scenario: Password change with incorrect current password
- **WHEN** authenticated user enters incorrect current password
- **THEN** system displays error message
- **AND** password is not changed

### Requirement: User can view and edit their profile
The system SHALL allow authenticated users to view and update their profile information including name and email.

#### Scenario: View profile
- **WHEN** authenticated user navigates to profile page
- **THEN** system displays user's current profile information

#### Scenario: Update profile successfully
- **WHEN** authenticated user updates their profile information
- **THEN** system saves the changes
- **AND** system displays success message

### Requirement: JWT tokens are issued upon login
The system SHALL issue JWT access tokens and refresh tokens upon successful authentication to support stateless API authentication.

#### Scenario: JWT tokens issued on login
- **WHEN** user logs in successfully
- **THEN** system generates a JWT access token with user claims
- **AND** system generates a refresh token
- **AND** tokens are returned to the client

#### Scenario: Access token expiration
- **WHEN** user presents an expired access token
- **THEN** system rejects the request
- **AND** client should use refresh token to obtain new access token

#### Scenario: Refresh token rotation
- **WHEN** user presents a valid refresh token to obtain new tokens
- **THEN** system issues new access and refresh tokens
- **AND** the old refresh token is invalidated
