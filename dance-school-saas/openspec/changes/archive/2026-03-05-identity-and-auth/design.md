## Context

The dance school platform needs user authentication to secure tenant data and enable personalized experiences. 
The project already has partial ASP.NET Core Identity infrastructure in place (AppUser, AppRole, AppRefreshToken entities, IdentitySetupExtensions). 
This design outlines how to complete the authentication system with JWT support, password management, and role-based authorization.

## Goals / Non-Goals

**Goals:**
- Implement complete user registration with email/password
- Add login/logout with session management using JWT refresh tokens
- Configure role-based authorization for system and company roles
- Support multi-company user associations (users belong to multiple dance schools)
- Enable password reset via email tokens
- Integrate authentication with existing tenant resolution middleware

**Non-Goals:**
- Social login (Google, Facebook)
- Two-factor authentication (2FA)
- External identity providers
- REST API authentication layer (only MVC controllers)
- Email delivery implementation (stub for now)

## Decisions

### 1. JWT Token Strategy
**Decision:** Use tokens JWT access with refresh token rotation

**Rationale:** 
- ASP.NET Core Identity's default cookie authentication works for MVC, but JWT provides better scalability for future API needs
- Refresh tokens allow persistent sessions without re-login
- Token rotation (invalidating old refresh token on use) enhances security

**Alternatives Considered:**
- Cookie-only authentication: Simpler but limits future API use
- Permanent tokens: Security risk if compromised

### 2. Password Reset Flow
**Decision:** Use ASP.NET Core Identity's built-in token providers with email-based reset

**Rationale:**
- Built-in token generation handles security (time-limited, single-use)
- Email-based reset is standard user expectation
- Can swap email provider later without code changes

### 3. Multi-Company User Handling
**Decision:** Users authenticate once, then select active company from their associated companies

**Rationale:**
- Single sign-on across companies reduces friction
- PreferredCompanyId on AppUser provides default selection
- CompanyUser join table tracks active associations

**Alternatives Considered:**
- Separate login per company: Creates account management complexity
- Automatic company detection: Not feasible when user belongs to multiple

### 4. Role Structure
**Decision:** Two-tier roles - System roles (global) and Company roles (per-tenant)

**Rationale:**
- SystemAdmin needs access across all companies
- Company roles isolate permissions within each tenant
- CompanyUserRole join table enables per-company role assignment

## Risks / Trade-offs

1. **[Risk]** JWT token storage in local storage vs cookies
   - **Mitigation:** Use HTTP-only cookies for refresh tokens; access token in memory only

2. **[Risk]** Email service not implemented
   - **Mitigation:** Log reset links to console in development; stub interface for future SMTP integration

3. **[Risk]** Session concurrency (user logged in multiple devices)
   - **Mitigation:** Refresh token rotation invalidates previous token; consider device tracking for advanced scenarios

4. **[Risk]** Tenant context not available during login
   - **Mitigation:** Allow login without tenant; redirect to company selection after auth

## Migration Plan

1. **Phase 1: Core Identity**
   - Update AppDbContext to fully integrate IdentityDbContext
   - Add migration for Identity tables
   - Configure Identity options (password policy, lockout)

2. **Phase 2: Authentication Flow**
   - Implement AccountController with Register/Login/Logout actions
   - Create Razor views for login/register pages
   - Add JWT token generation service

3. **Phase 3: Authorization**
   - Configure authorization policies for each role
   - Add [Authorize] attributes to controllers
   - Implement company role assignment UI

4. **Phase 4: Polish**
   - Password reset flow
   - Profile management
   - Session persistence ("Remember me")

**Rollback:** Roll forward only - authentication changes are additive. Previous state had no login, so no rollback needed.

## Open Questions

1. Should we require email confirmation before first login? (Currently disabled for simplicity)
2. How to handle company invitation - should it create user account or link existing?
3. Session timeout duration - currently not configured, use defaults (14 days for persistent, 30 min for sliding)
