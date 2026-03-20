using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class CompanyController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITenantContext _tenantContext;

    public CompanyController(
        AppDbContext context,
        UserManager<AppUser> userManager,
        ITenantContext tenantContext)
    {
        _context = context;
        _userManager = userManager;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> Select()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get all companies for this user
        var userCompanies = await _context.CompanyUsers
            .Where(cu => cu.AppUserId == user.Id && cu.IsActive)
            .Include(cu => cu.Company)
            .Select(cu => cu.Company!)
            .Where(c => c.IsActive)
            .ToListAsync();

        if (userCompanies.Count == 0)
        {
            return RedirectToAction("Index", "Home");
        }

        // If user has only one company, redirect to it
        if (userCompanies.Count == 1)
        {
            var company = userCompanies.First();
            user.PreferredCompanyId = company.Id;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Home", new { area = company.Slug });
        }

        // Show company selection view
        return View(userCompanies);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Switch(Guid companyId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Verify user has access to this company
        var companyUser = await _context.CompanyUsers
            .Include(cu => cu.Company)
            .FirstOrDefaultAsync(cu =>
                cu.AppUserId == user.Id &&
                cu.CompanyId == companyId &&
                cu.IsActive);

        if (companyUser?.Company == null || !companyUser.Company.IsActive)
        {
            TempData["ErrorMessage"] = "You don't have access to this company.";
            return RedirectToAction("Select");
        }

        // Update preferred company
        user.PreferredCompanyId = companyId;
        await _userManager.UpdateAsync(user);

        // Redirect to the company's area
        return RedirectToAction("Index", "Home", new { area = companyUser.Company.Slug });
    }
}
