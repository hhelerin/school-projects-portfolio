using App.Domain;

namespace App.Domain.Identity;

public class AppRefreshToken : BaseEntity
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string? DeviceInfo { get; set; }
    public bool IsRevoked { get; set; } = false;
    public string? PreviousToken { get; set; } // For token rotation tracking
}