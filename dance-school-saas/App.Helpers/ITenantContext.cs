namespace App.Helpers;

public interface ITenantContext
{
    Guid CompanyId { get; set; }
    string Slug { get; set; }
}