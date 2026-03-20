namespace App.Domain;

public class Studio : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public string? ContactInfo { get; set; }
    
    public ICollection<StudioRoom> Rooms { get; set; } = new List<StudioRoom>();
}