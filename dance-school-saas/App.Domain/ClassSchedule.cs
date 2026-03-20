namespace App.Domain;

public class ClassSchedule : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = default!;
    
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    public Guid StudioRoomId { get; set; }
    public StudioRoom StudioRoom { get; set; } = default!;
    
    public bool IsCancelled { get; set; } = false;
    public string? CancellationReason { get; set; }
    
    public bool IsException { get; set; } = false;
    
    public string? Details { get; set; }
}
