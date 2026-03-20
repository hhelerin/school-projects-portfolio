namespace App.Domain;

public class StudioRoom : BaseEntity
{
    public Guid StudioId { get; set; }
    public Studio Studio { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    
    public ICollection<StudioFeature> Features { get; set; } = new List<StudioFeature>();
}