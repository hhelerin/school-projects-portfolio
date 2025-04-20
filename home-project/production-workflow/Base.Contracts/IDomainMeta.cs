namespace Contracts;

public interface IDomainMeta
{
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    // hidden information about the record in db
    public string? SysNotes { get; set; }
}
