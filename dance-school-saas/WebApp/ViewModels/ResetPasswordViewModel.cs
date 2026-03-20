using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = default!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;

    [Required]
    public string Token { get; set; } = default!;
}
