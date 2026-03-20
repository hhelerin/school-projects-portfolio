using App.DAL.EF;
using App.Domain.Identity;
using App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly RefreshTokenService _refreshTokenService;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context,
        JwtService jwtService,
        RefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Assign default role
            await _userManager.AddToRoleAsync(user, "CompanyEmployee");

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Generate JWT tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);

            // Store tokens in session or return to client
            HttpContext.Session.SetString("AccessToken", accessToken);
            HttpContext.Session.SetString("RefreshToken", refreshToken.RefreshToken);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Generate JWT tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);

            // Store tokens in session
            HttpContext.Session.SetString("AccessToken", accessToken);
            HttpContext.Session.SetString("RefreshToken", refreshToken.RefreshToken);

            // Redirect to preferred company dashboard if set
            if (user.PreferredCompanyId.HasValue)
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == user.PreferredCompanyId.Value);
                if (company != null)
                {
                    return Redirect($"/{company.Slug}/dashboard");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Account is locked out.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Revoke refresh token
        var refreshToken = HttpContext.Session.GetString("RefreshToken");
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        // Sign out
        await _signInManager.SignOutAsync();

        // Clear session
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    #region Password Management

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
        {
            // Revoke all refresh tokens on password change for security
            await _refreshTokenService.RevokeUserTokensAsync(user.Id);

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Account",
            new { token, email = user.Email }, Request.Scheme);

        // Log the reset link to console (email service stub)
        Console.WriteLine($"[EMAIL SERVICE] Password reset link for {user.Email}: {resetLink}");

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            return BadRequest("A code and email must be supplied for password reset.");
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
        {
            // Revoke all refresh tokens on password reset for security
            await _refreshTokenService.RevokeUserTokensAsync(user.Id);

            return RedirectToAction("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    #endregion

    #region User Profile Management

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Update user properties
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;

        // Handle email change if different
        if (user.Email != model.Email)
        {
            var emailChangeResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!emailChangeResult.Succeeded)
            {
                foreach (var error in emailChangeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Update username to match email
            await _userManager.SetUserNameAsync(user, model.Email);
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            // Refresh sign-in to update claims
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    #endregion

    #region Access Denied

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion
}