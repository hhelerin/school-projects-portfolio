namespace App.DTO.v1.Students;

public class RegisterStudentInputModel
{
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public string? Details { get; set; }
    public Guid? AppUserId { get; set; }
    public bool ConfirmDuplicate { get; set; }
}

public class UpdateStudentInputModel
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public string? Details { get; set; }
    public Guid? AppUserId { get; set; }
}
