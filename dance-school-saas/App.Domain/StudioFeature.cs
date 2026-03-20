namespace App.Domain;

public class StudioFeature : BaseEntity
{
    public Guid StudioRoomId { get; set; }
    public StudioRoom StudioRoom { get; set; } = default!;
    
    public Guid FeatureId { get; set; }
    public Feature Feature { get; set; } = default!;
    
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Details { get; set; }
}