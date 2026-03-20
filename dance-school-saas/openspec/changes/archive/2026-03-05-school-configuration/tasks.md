## 1. Domain Entities (App.Domain)

- [x] 1.1 Create `DanceStyle` entity with Id, CompanyId, Name, Details, IsDeleted
- [x] 1.2 Create `Level` entity with Id, CompanyId, Name, Details, IsDeleted
- [x] 1.3 Create `Feature` entity with Id, Name, Details (non-tenant-scoped)
- [x] 1.4 Create `Studio` entity with Id, CompanyId, Name, Details, ContactInfo, IsDeleted
- [x] 1.5 Create `StudioRoom` entity with Id, StudioId, Name, Details, IsDeleted
- [x] 1.6 Create `StudioFeature` entity with Id, StudioRoomId, FeatureId, ValidFrom, ValidUntil, Details
- [x] 1.7 Create `Instructor` entity with Id, CompanyId, Name, PersonalId, ContactInfo, AppUserId, Details, IsDeleted

## 2. EF Core Configuration (App.DAL.EF)

- [x] 2.1 Create `DanceStyleConfiguration` with CompanyId index
- [x] 2.2 Create `LevelConfiguration` with CompanyId index
- [x] 2.3 Create `FeatureConfiguration` with seed data (6 features)
- [x] 2.4 Create `StudioConfiguration` with CompanyId index
- [x] 2.5 Create `StudioRoomConfiguration` with StudioId FK and restrict delete
- [x] 2.6 Create `StudioFeatureConfiguration` with FKs to StudioRoom and Feature
- [x] 2.7 Create `InstructorConfiguration` with CompanyId and AppUserId indexes
- [x] 2.8 Update `AppDbContext` to add DbSets and apply configurations
- [x] 2.9 Generate EF Core migration `SchoolConfiguration`
- [x] 2.10 Apply migration to database

## 3. DTOs (App.DTO)

- [x] 3.1 Create `DanceStyleListItemDto`, `DanceStyleDetailDto`, `CreateDanceStyleInputModel`, `UpdateDanceStyleInputModel`
- [x] 3.2 Create `LevelListItemDto`, `LevelDetailDto`, `CreateLevelInputModel`, `UpdateLevelInputModel`
- [x] 3.3 Create `StudioListItemDto` (with room count), `StudioDetailDto`, `CreateStudioInputModel`, `UpdateStudioInputModel`
- [x] 3.4 Create `StudioRoomListItemDto`, `StudioRoomDetailDto`, `CreateStudioRoomInputModel`, `UpdateStudioRoomInputModel`
- [x] 3.5 Create `FeatureDto` for dropdown selection
- [x] 3.6 Create `InstructorListItemDto`, `InstructorDetailDto`, `CreateInstructorInputModel`, `UpdateInstructorInputModel`

## 4. CQRS Handlers - DanceStyles (App.BLL)

- [x] 4.1 Create `GetListDanceStyleQuery` and handler
- [x] 4.2 Create `GetDanceStyleByIdQuery` and handler
- [x] 4.3 Create `CreateDanceStyleCommand`, handler, and validator
- [x] 4.4 Create `UpdateDanceStyleCommand`, handler, and validator
- [x] 4.5 Create `DeleteDanceStyleCommand` and handler

## 5. CQRS Handlers - Levels (App.BLL)

- [x] 5.1 Create `GetListLevelQuery` and handler
- [x] 5.2 Create `GetLevelByIdQuery` and handler
- [x] 5.3 Create `CreateLevelCommand`, handler, and validator
- [x] 5.4 Create `UpdateLevelCommand`, handler, and validator
- [x] 5.5 Create `DeleteLevelCommand` and handler

## 6. CQRS Handlers - Studios (App.BLL)

- [x] 6.1 Create `GetListStudioQuery` and handler (include room count)
- [x] 6.2 Create `GetStudioByIdQuery` and handler (include rooms)
- [x] 6.3 Create `CreateStudioCommand`, handler, and validator
- [x] 6.4 Create `UpdateStudioCommand`, handler, and validator
- [x] 6.5 Create `DeleteStudioCommand` and handler (check for rooms)

## 7. CQRS Handlers - StudioRooms (App.BLL)

- [x] 7.1 Create `GetRoomsByStudioQuery` and handler
- [x] 7.2 Create `GetStudioRoomByIdQuery` and handler (include features)
- [x] 7.3 Create `CreateStudioRoomCommand`, handler, and validator
- [x] 7.4 Create `UpdateStudioRoomCommand`, handler, and validator
- [x] 7.5 Create `DeleteStudioRoomCommand` and handler
- [x] 7.6 Create `AddStudioFeatureCommand`, handler, and validator
- [x] 7.7 Create `RemoveStudioFeatureCommand` and handler

## 8. CQRS Handlers - Instructors (App.BLL)

- [x] 8.1 Create `GetListInstructorQuery` and handler
- [x] 8.2 Create `GetInstructorByIdQuery` and handler
- [x] 8.3 Create `CreateInstructorCommand`, handler, and validator
- [x] 8.4 Create `UpdateInstructorCommand`, handler, and validator
- [x] 8.5 Create `DeleteInstructorCommand` and handler
- [x] 8.6 Create `GetCompanyUsersForDropdownQuery` for instructor form

## 9. MVC Controllers (WebApp)

- [x] 9.1 Create `DanceStyleController` with Index, Create, Edit, Delete actions
- [x] 9.2 Create `LevelController` with Index, Create, Edit, Delete actions
- [x] 9.3 Create `StudioController` with Index, Detail, Create, Edit, Delete actions
- [x] 9.4 Create `StudioRoomController` with Index, Create, Edit, Delete, AddFeature, RemoveFeature actions
- [x] 9.5 Create `InstructorController` with Index, Create, Edit, Delete actions

## 10. Views (WebApp)

- [x] 10.1 Create DanceStyles/Index.cshtml with table and Create/Edit/Delete buttons
- [x] 10.2 Create DanceStyles/Create.cshtml with form
- [x] 10.3 Create DanceStyles/Edit.cshtml with pre-filled form
- [x] 10.4 Create Levels/Index.cshtml with table
- [x] 10.5 Create Levels/Create.cshtml with form
- [x] 10.6 Create Levels/Edit.cshtml with pre-filled form
- [x] 10.7 Create Studios/Index.cshtml with table and room counts
- [x] 10.8 Create Studios/Detail.cshtml with studio info and rooms list
- [x] 10.9 Create Studios/Create.cshtml with form
- [x] 10.10 Create Studios/Edit.cshtml with pre-filled form
- [x] 10.11 Create StudioRooms/Index.cshtml with rooms table
- [x] 10.12 Create StudioRooms/Create.cshtml with form
- [x] 10.13 Create StudioRooms/Edit.cshtml with form and features management
- [x] 10.14 Create Instructors/Index.cshtml with table
- [x] 10.15 Create Instructors/Create.cshtml with form and AppUser dropdown
- [x] 10.16 Create Instructors/Edit.cshtml with pre-filled form

## 11. Navigation and Authorization

- [x] 11.1 Update `_Layout.cshtml` or Dashboard with "Configuration" nav section
- [x] 11.2 Add Dance Styles nav link (visible to CompanyUser+)
- [x] 11.3 Add Levels nav link (visible to CompanyUser+)
- [x] 11.4 Add Studios nav link (visible to CompanyUser+)
- [x] 11.5 Add Instructors nav link (visible to CompanyUser+)
- [x] 11.6 Apply `[Authorize]` with CompanyUser role to all configuration routes
- [x] 11.7 Apply `[Authorize]` with CompanyAdmin/CompanyManager to Create/Edit/Delete actions

## 12. Tests (WebApp.Tests)

- [x] 12.1 Create `CreateDanceStyleCommandTests` - creates style scoped to correct tenant
- [x] 12.2 Create `DeleteDanceStyleCommandTests` - sets IsDeleted, does not hard delete
- [x] 12.3 Create `GetListDanceStyleQueryTests` - only returns styles for current tenant
- [x] 12.4 Create `AddStudioFeatureCommandTests` - creates join record with correct dates
- [x] 12.5 Create `RemoveStudioFeatureCommandTests` - removes join record
- [x] 12.6 Create `CreateInstructorCommandTests` - valid input creates instructor under tenant
