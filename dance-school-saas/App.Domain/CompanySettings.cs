namespace App.Domain;

public class CompanySettings : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}