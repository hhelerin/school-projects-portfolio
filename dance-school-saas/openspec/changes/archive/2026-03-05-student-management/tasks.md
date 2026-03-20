## 1. Domain Entity

- [x] 1.1 Create Student entity in App.Domain/Student.cs with Id, CompanyId, Name, PersonalId, ContactInfo, Details, AppUserId (nullable Guid), IsDeleted
- [x] 1.2 Configure Student in App.DAL.EF/Configurations/StudentConfiguration.cs with indexes (CompanyId, AppUserId, CompanyId+Name)

## 2. Helper Classes

- [x] 2.1 Add PagedResult<T> helper to App.Helpers/PagedResult.cs

## 3. DTOs

- [x] 3.1 Create App.DTO/v1/Students/StudentDtos.cs with StudentListItemDto and StudentDetailDto
- [x] 3.2 Create App.DTO/v1/Students/StudentInputModels.cs with RegisterStudentInputModel and UpdateStudentInputModel
- [x] 3.3 Create placeholder DTOs: PackageSummaryDto, TrialRecordSummaryDto, ShowcaseEligibilitySummaryDto

## 4. BLL Commands

- [x] 4.1 Create App.BLL/Features/Students/RegisterStudentCommand.cs with duplicate detection and ConfirmDuplicate support
- [x] 4.2 Create App.BLL/Features/Students/UpdateStudentCommand.cs with AppUser validation
- [x] 4.3 Create App.BLL/Features/Students/DeleteStudentCommand.cs with soft delete and active package check
- [x] 4.4 Create FluentValidation validators for all commands

## 5. BLL Queries

- [x] 5.1 Create App.BLL/Features/Students/GetStudentListQuery.cs with search and pagination
- [x] 5.2 Create App.BLL/Features/Students/GetStudentByIdQuery.cs

## 6. Controller

- [x] 6.1 Create WebApp/Controllers/StudentController.cs with CRUD endpoints at /{slug}/students/
- [x] 6.2 Implement authorization (CompanyEmployee+ for CRUD, Admin/Owner for delete)
- [x] 6.3 Implement duplicate warning handling in Create POST

## 7. Views

- [x] 7.1 Create WebApp/Views/Students/Index.cshtml with search bar, table, pagination
- [x] 7.2 Create WebApp/Views/Students/Create.cshtml with form and duplicate warning
- [x] 7.3 Create WebApp/Views/Students/Detail.cshtml with info card and placeholder sections
- [x] 7.4 Create WebApp/Views/Students/Edit.cshtml with pre-filled form

## 8. Navigation

- [x] 8.1 Add "Students" link to _Layout.cshtml under Operations section
- [x] 8.2 Enable "Students" link on the "Students" card on the company dashboard

## 9. EF Core

- [x] 9.1 Add migration "StudentManagement" with dotnet ef migrations add
- [x] 9.2 Apply migration with dotnet ef database update

## 10. Tests

- [x] 10.1 Create WebApp.Tests/Students/RegisterStudentCommandTests.cs
- [x] 10.2 Create WebApp.Tests/Students/UpdateStudentCommandTests.cs
- [x] 10.3 Create WebApp.Tests/Students/DeleteStudentCommandTests.cs
- [x] 10.4 Create WebApp.Tests/Students/GetStudentListQueryTests.cs
