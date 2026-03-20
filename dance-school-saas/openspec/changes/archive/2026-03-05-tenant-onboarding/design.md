## Context

The identity-and-auth change established the foundation for authentication, authorization, and user management. 
Now we need to enable dance school owners to self-register their schools and onboard their staff without manual platform administrator intervention.

Current state:
- Identity system with AppUser, Company, CompanyUser, CompanyUserRole entities exists
- Authentication middleware and tenant resolution are operational
- Company roles (CompanyOwner, CompanyAdmin, etc.) are defined
- No public registration endpoint for new schools
- No staff invitation workflow exists

## Goals / Non-Goals

**Goals:**
- Enable new school owners to register their school via a public-facing form
- Create all necessary records (Company, AppUser, CompanyUser, CompanyUserRole) atomically
- If AppUser exists, connect the Company info
- Automatically authenticate the owner and redirect to their new school dashboard
- Allow CompanyAdmins to invite staff members by creating and showing a registration-link
- Handle both existing AppUser accounts and new user creation during invitation
- Provide immediate visual feedback with slug preview during registration

**Non-Goals:**
- Email service integration (temp passwords logged to console only)
- School settings configuration UI
- Staff management screens (list, edit, deactivate)
- Role management CRUD
- Subscription tier selection beyond default "Free"
- Custom domain configuration
- Bulk staff import

## Decisions

**1. Single Transaction for School Registration**
All four creation steps (Company, AppUser, CompanyUser, CompanyUserRole) happen in one UnitOfWork transaction. 
If any step fails, the entire operation rolls back to avoid orphaned records.
- Rationale: Data consistency is critical; partial creation would leave the system in an invalid state
- Alternative considered: Separate API calls for each step — rejected due to complexity and failure modes

**2. Slug Auto-Generation with User Override**
The slug is auto-generated from SchoolName (lowercase, spaces→hyphens, special chars removed) but users can edit it. 
Real-time client-side preview shows the final URL.
- Rationale: Reduces friction while giving users control over their URL
- Alternative considered: Random generated slugs — rejected as they're not memorable or brandable

**3. Case-Insensitive Slug Uniqueness**
Slug uniqueness check is case-insensitive to avoid confusion ("MySchool" vs "myschool").
- Rationale: Path-based routing should be case-insensitive for usability
- Trade-off: Prevents users from having visually similar but technically different slugs

**4. Staff Invitation: Existing vs New User Paths**
When inviting staff:
- If email exists: Create CompanyUser + CompanyUserRole only
- If email doesn't exist: Create AppUser (with temp password), CompanyUser, CompanyUserRole
- Rationale: Supports both onboarding new hires and adding existing platform users
- Trade-off: Temporary passwords are logged to console rather than emailed (acknowledged limitation)

**5. Immediate Sign-In After Registration**
After successful school registration, the owner is automatically signed in.
- Rationale: Reduces friction; owner is already authenticated via form submission
- Alternative considered: Require email verification first — rejected to streamline onboarding

**6. Path-Based Authorization for Staff Invitation**
Staff invitation endpoints are scoped under `/{slug}/staff/invite` and use the existing tenant resolution middleware.
- Rationale: Leverages existing infrastructure; authorization checks CompanyUser roles via the resolved tenant
- Trade-off: Requires authenticated user to have a CompanyUser record for the target company

## Risks / Trade-offs

**Risk:** Slug collisions could frustrate users if their preferred slug is taken.
→ Mitigation: Real-time availability check with helpful messaging; suggest alternatives based on their input.

**Risk:** Staff invitation creates AppUser with weak temporary password.
→ Mitigation: Force password change on first login (leveraged from existing identity flow); password logged only to console in development.

**Risk:** No email verification could allow spam registrations.
→ Mitigation: Acceptable for practice project; CAPTCHA or email verification would be added for production.

**Risk:** Database transaction timeout during school creation if under heavy load.
→ Mitigation: Keep transaction scope minimal; operation involves only 4 simple inserts. Monitor and optimize if needed.

**Risk:** User invites staff to wrong company (cross-tenant issue).
→ Mitigation: Tenant resolution middleware ensures all operations are scoped to the URL slug; authorization policy verifies user has admin role for that specific company.

## Migration Plan

Not applicable — this is a new feature with no existing data to migrate.

## Open Questions

None — all technical decisions resolved.
