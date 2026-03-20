using App.Domain.Identity;

namespace App.Domain;

public class CompanyUser : BaseEntity, ITenantEntity
{
    public Guid AppUserId { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsActive { get; set; }
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public AppUser? AppUser { get; set; }
    public Company? Company { get; set; }
}