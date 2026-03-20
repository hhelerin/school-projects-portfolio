namespace App.Domain;

public class CompanyRole : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSystemProtected { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public Company? Company { get; set; }
    public ICollection<CompanyUserRole>? CompanyUserRoles { get; set; }
}