using System.ComponentModel.DataAnnotations;
using Base.Domain.Identity;
using Domain.Identity;

namespace App.Domain.Identity;

public class AppUser : BaseUser<AppUserRole>
{
    [Display(Name = nameof(AppUser), Prompt = nameof(AppUser), ResourceType = typeof(Base.Resources.Common))]
    public ICollection<CustomersUsers>? CustomersUsers { get; set; }
    
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}