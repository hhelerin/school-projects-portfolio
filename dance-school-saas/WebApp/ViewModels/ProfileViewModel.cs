using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = default!;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = default!;

    [Display(Name = "Phone Number")]
    [Phone]
    public string? PhoneNumber { get; set; }
}
