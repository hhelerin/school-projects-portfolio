namespace App.DTO.v1.Identity;

public class InviteStaffInputModel
{
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public Guid CompanyRoleId { get; set; }
}

public class InviteStaffResult
{
    public Guid UserId { get; set; }
}

public class StaffInvitationViewModel
{
    public List<CompanyRoleDto> CompanyRoles { get; set; } = new();
}

public class CompanyRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
