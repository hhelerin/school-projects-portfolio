using App.Domain.Identity;
using Base.Domain.Identity;

namespace Domain.Identity;

public class AppUserRole  : BaseUserRole<AppUser, AppRole>
{
    public Guid AppUserId { get; set; }
    
    public AppUser? AppUser { get; set; }
    
    public Guid AppRoleId { get; set; }
    
    public AppRole? AppRole{ get; set; }
}