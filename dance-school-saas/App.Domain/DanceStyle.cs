namespace App.Domain;

public class DanceStyle : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}