## Context

This change implements student management for the dance school platform. Students are core to the business - 
they attend classes, purchase packages, and participate in showcases. This is Change 6 of 13, building on 
previous changes for project scaffold, identity/auth, tenant onboarding, school configuration, and class scheduling.

The Student entity links to AppUser for optional platform account access, enabling walk-in students 
(registered by staff without login) vs. students with platform accounts.

## Goals / Non-Goals

**Goals:**
- Full CRUD operations for student records with tenant isolation
- Support for walk-in students (no AppUser) and platform-linked students
- Duplicate detection with override capability
- Soft delete with active package validation
- Search and pagination for student list
- Role-based authorization following existing patterns

**Non-Goals:**
- Package selling functionality (Change 7)
- Trial record management (Change 8)
- Attendance tracking display (Change 8)
- Showcase eligibility management (Change 9)
- Student self-registration portal
- Bulk student import

## Decisions

### 1. Duplicate Detection Strategy

**Decision:** Check for duplicate based on Name + ContactInfo combination, return warning (not hard-block) to allow 
staff to confirm or override via ConfirmDuplicate flag.

**Rationale:** Dance schools may have legitimate reasons to register the same person twice (e.g., different 
family members with similar names, or re-enrollment after a long break). Returning a warning lets staff decide rather 
than blocking registration entirely.

**Alternative considered:** Hard-block duplicates → Rejected because staff should have final say.

### 2. Soft Delete with Active Package Check

**Decision:** Prevent soft delete if Student has any active (non-expired, non-deleted) Package records. 
Return clear error message.

**Rationale:** Deleting a student with active packages would leave orphaned data and cause issues at class check-in. 
This business rule prevents data integrity issues.

**Alternative considered:** Cascade delete packages → Rejected - packages represent billable items that should persist 
for financial records.

### 3. PagedResult Helper Location

**Decision:** Add PagedResult<T> to App.Helpers namespace.

**Rationale:** This is a generic helper used across multiple features. Following the pattern established in the codebase, helpers go in App.Helpers.

### 4. Placeholder Sections for Future Changes

**Decision:** Include empty placeholder sections in StudentDetailDto for Packages, TrialRecords, Attendance, and ShowcaseEligibility - populated in Changes 7, 8, and 9 respectively.

**Rationale:** The detail view needs structure now to avoid layout changes later. Placeholder cards with "Coming soon" text provide visual consistency.

## Risks / Trade-offs

- **[Risk]** AppUser link validation complexity → **Mitigation:** Validate both existence and that AppUser isn't already linked to another student in the same tenant.
- **[Risk]** Pagination performance with large student lists → **Mitigation:** Use EF Core Skip/Take with proper indexing on CompanyId + Name.
- **[Risk]** Duplicate detection false positives (common names) → **Mitigation:** ConfirmDuplicate flag allows staff to override.
- **[Trade-off]** Including placeholder DTOs adds minor overhead now but reduces future view changes.
