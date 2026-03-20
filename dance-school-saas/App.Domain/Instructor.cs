namespace App.Domain;

public class Instructor : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public Guid? AppUserId { get; set; }
    public string? Details { get; set; }
}