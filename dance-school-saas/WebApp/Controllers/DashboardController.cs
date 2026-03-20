using App.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[Route("{slug}/dashboard")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /{slug}/dashboard
    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        // Get the company
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
        
        if (company == null)
        {
            return NotFound();
        }

        // Get current user
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized();
        }

        // Check if user is a member of this company
        var appUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userEmail);
        
        if (appUser == null)
        {
            return Unauthorized();
        }

        var isMember = await _context.CompanyUsers
            .AnyAsync(cu => cu.AppUserId == appUser.Id && cu.CompanyId == company.Id && cu.IsActive);
        
        if (!isMember)
        {
            return Forbid();
        }

        // Pass data to view
        ViewData["CompanyName"] = company.Name;
        ViewData["CompanySlug"] = company.Slug;
        ViewData["UserFirstName"] = appUser.FirstName;

        return View();
    }
}
