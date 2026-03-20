namespace App.Helpers;

public class TenantContext: ITenantContext
{
    public Guid CompanyId { get; set; }
    public string Slug { get; set; } = string.Empty;
}