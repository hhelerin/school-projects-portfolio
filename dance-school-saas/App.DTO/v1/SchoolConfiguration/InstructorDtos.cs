namespace App.DTO.v1.SchoolConfiguration;

public class InstructorListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool HasPortalAccess => AppUserId.HasValue;
    public Guid? AppUserId { get; set; }
}

public class InstructorDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public Guid? AppUserId { get; set; }
    public string? AppUserEmail { get; set; }
    public string? Details { get; set; }
}

public class CreateInstructorInputModel
{
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public Guid? AppUserId { get; set; }
    public string? Details { get; set; }
}

public class UpdateInstructorInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public Guid? AppUserId { get; set; }
    public string? Details { get; set; }
}

public class CompanyUserDropdownDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
}