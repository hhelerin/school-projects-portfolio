## Why

Dance schools need to manage their students - registering new students, viewing profiles, editing their information, and handling soft deletes. This is essential for day-to-day operations and forms the foundation for tracking attendance, packages, and trial records in future changes.

## What Changes

- **New Student Entity**: App.Domain/Student.cs with Id, CompanyId, Name, PersonalId (optional), ContactInfo (optional), Details (optional), AppUserId (nullable FK), IsDeleted
- **PagedResult Helper**: App.Helpers/PagedResult.cs for pagination support
- **BLL Commands**: RegisterStudentCommand (with duplicate detection), UpdateStudentCommand, DeleteStudentCommand (soft delete with active package check)
- **BLL Queries**: GetStudentListQuery (with search and pagination), GetStudentByIdQuery
- **DTOs**: StudentListItemDto, StudentDetailDto, RegisterStudentInputModel, UpdateStudentInputModel, placeholder DTOs for packages/trials/showcase
- **StudentController**: RESTful MVC controller at /{slug}/students/ with full CRUD operations
- **Views**: Index.cshtml (search + pagination), Create.cshtml (with duplicate warning), Detail.cshtml, Edit.cshtml
- **Navigation**: Add "Students" link under Operations section in layout
- **Authorization**: Role-based access (CompanyEmployee+ for CRUD, Admin/Owner for delete)
- **EF Core**: StudentConfiguration with indexes, migration "StudentManagement"
- **Tests**: Unit tests for all commands and queries

## Capabilities

### New Capabilities
- `student-management`: Full student CRUD operations including registration, profile viewing, editing, and soft delete with duplicate detection and active package validation

### Modified Capabilities
- (none - this is a new capability)

## Impact

- New files created in: App.Domain, App.BLL/Features/Students, App.DTO/v1/Students, WebApp/Controllers, WebApp/Views/Students
- No breaking changes to existing functionality
- Requires EF Core migration
- Depends on existing AppUser entity for optional linking
