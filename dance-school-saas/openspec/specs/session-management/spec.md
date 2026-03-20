## ADDED Requirements

### Requirement: JWT access tokens are issued on login
The system SHALL generate JWT access tokens upon successful user authentication for stateless API authentication.

#### Scenario: JWT token generation on login
- **WHEN** user successfully logs in with valid credentials
- **THEN** system generates a JWT access token with user claims
- **AND** system generates a refresh token for token renewal
- **AND** tokens include expiration timestamps
- **AND** access token contains user ID, email, and role claims

#### Scenario: JWT token includes user identity
- **WHEN** system generates JWT access token
- **THEN** token includes user's unique identifier
- **AND** token includes user's email address
- **AND** token includes user's current company context
- **AND** token includes user's roles for the current company

### Requirement: Refresh tokens enable persistent sessions
The system SHALL use refresh tokens to allow users to maintain sessions without re-entering credentials.

#### Scenario: Refresh token generation
- **WHEN** user logs in successfully
- **THEN** system generates a long-lived refresh token
- **AND** refresh token is stored securely (HTTP-only cookie)
- **AND** refresh token is associated with the user session

#### Scenario: Token refresh with valid refresh token
- **WHEN** user presents a valid, non-expired refresh token
- **THEN** system generates new access and refresh tokens
- **AND** old refresh token is invalidated (rotation)
- **AND** new tokens have updated expiration times

#### Scenario: Token refresh with expired refresh token
- **WHEN** user presents an expired refresh token
- **THEN** system rejects the refresh request
- **AND** user must re-authenticate with credentials

#### Scenario: Token refresh with invalid refresh token
- **WHEN** user presents an invalid or tampered refresh token
- **THEN** system rejects the refresh request
- **AND** system logs the security event

### Requirement: Access tokens have appropriate expiration
The system SHALL set appropriate expiration times for access tokens to balance security and user experience.

#### Scenario: Access token expiration time
- **WHEN** system generates access token
- **THEN** token expires after 15 minutes (configurable)
- **AND** client must use refresh token to get new access token

#### Scenario: Access token expiration handling
- **WHEN** client presents expired access token
- **THEN** system returns 401 Unauthorized response
- **AND** client should automatically attempt token refresh

### Requirement: Secure token storage
The system SHALL ensure tokens are stored securely to prevent unauthorized access.

#### Scenario: Refresh token in HTTP-only cookie
- **WHEN** system issues refresh token to client
- **THEN** token is stored in HTTP-only cookie
- **AND** cookie is marked as secure (HTTPS only)
- **AND** cookie has appropriate SameSite settings

#### Scenario: Access token in memory
- **WHEN** client receives access token
- **THEN** token is stored in memory (not localStorage)
- **AND** token is not persisted across browser sessions

### Requirement: Token revocation and invalidation
The system SHALL provide mechanisms to invalidate tokens when security requires it.

#### Scenario: Logout invalidates tokens
- **WHEN** user logs out
- **THEN** current refresh token is invalidated
- **AND** any associated access tokens become invalid
- **AND** user must re-authenticate to access protected resources

#### Scenario: Password change invalidates tokens
- **WHEN** user changes their password
- **THEN** all existing refresh tokens for that user are invalidated
- **AND** user must re-authenticate with new password

#### Scenario: Account lockout invalidates tokens
- **WHEN** user account is locked due to security policy
- **THEN** all existing tokens for that user are invalidated
- **AND** user cannot access protected resources until account is unlocked

### Requirement: Multi-device session management
The system SHALL support multiple concurrent sessions while maintaining security.

#### Scenario: Multiple device login
- **WHEN** user logs in from a second device
- **THEN** both sessions are valid independently
- **AND** each device has its own refresh token
- **AND** actions on one device don't affect other devices

#### Scenario: Session-specific token invalidation
- **WHEN** user logs out from one device
- **THEN** only that device's tokens are invalidated
- **AND** other device sessions remain active

### Requirement: Token validation and claims
The system SHALL validate tokens and extract user information from token claims.

#### Scenario: Token signature validation
- **WHEN** system receives JWT token
- **THEN** system validates token signature using configured secret
- **AND** system validates token has not been tampered with

#### Scenario: Token expiration validation
- **WHEN** system processes JWT token
- **THEN** system checks token expiration timestamp
- **AND** system rejects expired tokens

#### Scenario: Token claims extraction
- **WHEN** system validates JWT token
- **THEN** system extracts user ID from token claims
- **AND** system extracts user roles from token claims
- **AND** system extracts company context from token claims

### Requirement: Remember me functionality
The system SHALL provide persistent login option for user convenience.

#### Scenario: Remember me on login
- **WHEN** user checks "Remember me" during login
- **THEN** refresh token has extended expiration (14 days)
- **AND** user remains logged in across browser sessions

#### Scenario: Standard session expiration
- **WHEN** user does not check "Remember me"
- **THEN** refresh token expires after 30 minutes of inactivity
- **AND** user must re-authenticate after session expires
