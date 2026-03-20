using App.Helpers;

namespace WebApp.Tests.Unit;

// Test implementation of ITenantContext for unit tests
public class TestTenantContext : ITenantContext
{
    public Guid CompanyId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Guid? UserId { get; set; }

    public TestTenantContext(Guid companyId)
    {
        CompanyId = companyId;
        Slug = "test-slug";
    }
}
