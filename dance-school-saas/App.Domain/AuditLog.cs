namespace App.Domain;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = default!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; }
}