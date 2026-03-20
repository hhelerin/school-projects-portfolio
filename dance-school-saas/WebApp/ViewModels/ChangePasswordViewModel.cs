using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = default!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = default!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;
}
