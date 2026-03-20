## 1. App.DTO - Input Models

- [x] 1.1 Create SignUpSchoolInputModel with OwnerFirstName, OwnerLastName, Email, Password, ConfirmPassword, SchoolName, Slug, RegistrationCode, VATCode
- [x] 1.2 Create SignUpSchoolResult DTO with CompanyId and Slug
- [x] 1.3 Create InviteStaffInputModel with Email, FirstName, LastName, CompanyRoleId
- [x] 1.4 Create InviteStaffResult DTO with UserId
- [x] 1.5 Create StaffInvitationViewModel with CompanyRoles list for dropdown

## 2. App.BLL - School Sign Up Command

- [x] 2.1 Create SignUpSchoolCommand handler structure
- [x] 2.2 Implement slug uniqueness validation (case-insensitive)
- [x] 2.3 Implement email uniqueness validation
- [x] 2.4 Implement Company creation with IsActive=true and SubscriptionTier=Free
- [x] 2.5 Implement AppUser creation for owner
- [x] 2.6 Implement CompanyUser creation with IsActive=true and JoinedAt
- [x] 2.7 Implement CompanyUserRole assignment with CompanyOwner role
- [x] 2.8 Wrap all operations in UnitOfWork transaction
- [x] 2.9 Return Result<SignUpSchoolResult> with appropriate error messages
- [x] 2.10 Create SignUpSchoolCommandValidator with FluentValidation

## 3. App.BLL - Staff Invitation Command

- [x] 3.1 Create InviteStaffCommand handler structure
- [x] 3.2 Implement check for existing AppUser by email
- [x] 3.3 Implement path for existing user: create CompanyUser + CompanyUserRole only
- [x] 3.4 Implement path for new user: create AppUser with temp password, CompanyUser, CompanyUserRole
- [x] 3.5 Implement temporary password generation and console logging
- [x] 3.6 Implement validation: check if user already member of company
- [x] 3.7 Return Result<InviteStaffResult> with appropriate error messages
- [x] 3.8 Create InviteStaffCommandValidator with FluentValidation

## 4. WebApp - Sign Up Controller and Views

- [x] 4.1 Create SignUpController with GET /register action
- [x] 4.2 Create Views/Registration/SignUp.cshtml with all form fields
- [x] 4.3 Implement client-side slug auto-generation from SchoolName (JS: spaces→hyphens, lowercase)
- [x] 4.4 Add Bootstrap styling to sign-up form
- [x] 4.5 Implement POST /signup action with SignUpSchoolCommand dispatch
- [x] 4.6 Implement automatic sign-in after successful registration
- [x] 4.7 Implement redirect to /{slug}/dashboard on success
- [x] 4.8 Implement form error display on validation failure
- [x] 4.9 Add SignUpSchoolInputModel validation attributes

## 5. WebApp - Staff Controller and Views

- [x] 5.1 Create StaffController with Authorize attribute for CompanyAdmin/CompanyOwner roles
- [x] 5.2 Implement GET /{slug}/staff/invite action with role dropdown population
- [x] 5.3 Create Views/Staff/Invite.cshtml with form fields and role dropdown
- [x] 5.4 Add Bootstrap styling to invitation form
- [x] 5.5 Implement POST /{slug}/staff/invite action with InviteStaffCommand dispatch
- [x] 5.6 Implement redirect to /{slug}/staff on success
- [x] 5.7 Implement form error display on validation failure
- [x] 5.8 Add InviteStaffInputModel validation attributes

## 6. WebApp - Dashboard Controller and View

- [x] 6.1 Create DashboardController with GET /{slug}/dashboard action
- [x] 6.2 Add Authorize attribute requiring authenticated user
- [x] 6.3 Implement company membership check
- [x] 6.4 Create Views/Dashboard/Index.cshtml with welcome message
- [x] 6.5 Display owner first name and school name in welcome message
- [x] 6.6 Add placeholder navigation links menu for future features
- [x] 6.7 Add Bootstrap styling to dashboard page

## 7. WebApp.Tests - Unit Tests

- [x] 7.1 Create test: SignUpSchoolCommand creates company, user, CompanyUser, and assigns CompanyOwner role
- [x] 7.2 Create test: SignUpSchoolCommand returns failure for duplicate slug
- [x] 7.3 Create test: SignUpSchoolCommand with existing user creates company, CompanyUser and assigns CompanyOwner role
- [x] 7.4 Create test: InviteStaffCommand for existing AppUser creates CompanyUser record only
- [x] 7.5 Create test: InviteStaffCommand for new AppUser creates user with temp password
- [x] 7.6 Create test: InviteStaffCommand returns failure if user already member of company
- [x] 7.7 Create test: InviteStaffCommand returns failure for invalid email format
