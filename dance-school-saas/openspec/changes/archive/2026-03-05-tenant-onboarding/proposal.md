## Why

Dance school owners need a self-service way to sign up their school and start using the platform immediately. 
Currently, there's no onboarding flow—schools cannot be created without manual intervention. 
Additionally, school administrators need to invite staff members and assign them appropriate roles within their organization.

## What Changes

- **New public registration page** (`/signup`) for dance school owners to sign up their school
- **School creation workflow** that creates Company, AppUser, CompanyUser, and assigns CompanyOwner role in a single transaction
- **Automatic sign-in** after registration with redirect to (preferred) school dashboard
- **Staff invitation system** for CompanyAdmins to invite new staff by creating a register-link with CompanyId and CompanyRole
- **Dashboard stub page** as the landing spot after registration
- **URL-friendly slug generation** from school name with client-side preview

## Capabilities

### New Capabilities

- `school-registration`: Self-service school creation with owner registration, slug validation, and automatic role assignment
- `staff-invitation`: Invite staff members by creating a link, handle existing vs new users 
(existing users get companyrole added to their appuser), assign company roles
- `dashboard-stub`: Basic dashboard landing page for school owners 

### Modified Capabilities

- None (this change builds on existing identity-and-auth infrastructure)

## Impact

- **WebApp**: New SignUpController, StaffController, DashboardController with associated views
- **App.BLL**: New MediatR commands (RegisterSchoolCommand, InviteStaffCommand) with handlers and validators
- **App.DTO**: New input models and result DTOs for sign-up and invitation flows
- **Database**: No schema changes—uses existing Company, AppUser, CompanyUser, CompanyUserRole entities
- **Authorization**: New policies for staff invitation (requires CompanyAdmin or CompanyOwner)
- **Testing**: Unit tests for command handlers covering success and failure scenarios
