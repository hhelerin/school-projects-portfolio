## Why

The dance school platform requires secure user authentication to protect tenant data and enable personalized experiences. Currently, there is no proper login system, which means students and staff cannot securely access their accounts, view their schedules, or manage their profiles. Implementing a robust authentication system is essential for multi-tenant data isolation and professional service delivery.

## What Changes

- Implement user registration with email/password for students and staff
- Add login/logout functionality with session management
- Integrate JWT tokens for API authentication (future-proofing)
- Set up password reset flow via email tokens
- Configure role-based authorization (SystemAdmin, CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee)
- Link users to companies via CompanyUser join table
- Add multi-company user support (users can belong to multiple dance schools)
- Implement tenant-aware authentication (users authenticate to their specific company)
- Add account lockout and security measures

## Capabilities

### New Capabilities
- `user-auth`: Complete user authentication system including registration, login, logout, password management, and JWT token support
- `company-user-roles`: Role assignment and management for company users (CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee)
- `session-management`: Session handling with refresh tokens and persistent login options

### Modified Capabilities
- (None - this is a new capability)

## Impact

- **App.Domain**: New entities for Identity (AppUser, AppRole, AppRefreshToken) - already partially defined
- **App.DAL.EF**: AppDbContext changes to integrate IdentityDbContext
- **WebApp**: New authentication controllers, views, and authorization middleware
- **Configuration**: appsettings.json updates for JWT settings
- **Dependencies**: Microsoft.AspNetCore.Identity.EntityFrameworkCore, System.IdentityModel.Tokens.Jwt
