using Base.Domain.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppRole : BaseRole<AppUserRole>
{
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}