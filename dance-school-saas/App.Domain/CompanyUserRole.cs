namespace App.Domain;

public class CompanyUserRole : BaseEntity
{
    public Guid CompanyUserId { get; set; }
    public Guid CompanyRoleId { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public CompanyUser? CompanyUser { get; set; }
    public CompanyRole? CompanyRole { get; set; }
}