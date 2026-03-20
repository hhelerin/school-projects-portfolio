using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = default!;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}