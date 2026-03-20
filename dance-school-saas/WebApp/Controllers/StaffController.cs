using App.BLL.Commands;
using App.DAL.EF;
using App.DTO.v1.Identity;
using App.Helpers;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyAdmin", "CompanyOwner")]
[Route("{slug}/staff")]
public class StaffController : Controller
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public StaffController(IMediator mediator, AppDbContext context, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _context = context;
        _currentUserService = currentUserService;
    }

    // GET: /{slug}/staff/invite
    [HttpGet("invite")]
    public async Task<IActionResult> Invite(string slug)
    {
        // Get current tenant
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
        
        if (company == null)
        {
            return NotFound();
        }

        // Get company roles for dropdown
        var roles = await _context.CompanyRoles
            .Where(r => r.CompanyId == company.Id)
            .OrderBy(r => r.Name)
            .Select(r => new CompanyRoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();

        var viewModel = new StaffInvitationViewModel
        {
            CompanyRoles = roles
        };

        ViewData["CompanySlug"] = slug;
        return View(viewModel);
    }

    // POST: /{slug}/staff/invite
    [HttpPost("invite")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Invite(string slug, InviteStaffInputModel model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate roles dropdown
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
            
            if (company != null)
            {
                var roles = await _context.CompanyRoles
                    .Where(r => r.CompanyId == company.Id)
                    .OrderBy(r => r.Name)
                    .Select(r => new CompanyRoleDto { Id = r.Id, Name = r.Name })
                    .ToListAsync();

                var viewModel = new StaffInvitationViewModel
                {
                    CompanyRoles = roles
                };
                ViewData["CompanySlug"] = slug;
                return View(viewModel);
            }
            
            return View(new StaffInvitationViewModel());
        }

        // Get company
        var targetCompany = await _context.Companies
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
        
        if (targetCompany == null)
        {
            return NotFound();
        }

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var command = new InviteStaffCommand(model, targetCompany.Id, currentUserId.Value);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Staff member invited successfully!";
            return Redirect($"/{slug}/staff");
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        
        // Repopulate roles dropdown
        var companyRoles = await _context.CompanyRoles
            .Where(r => r.CompanyId == targetCompany.Id)
            .OrderBy(r => r.Name)
            .Select(r => new CompanyRoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();

        var errorViewModel = new StaffInvitationViewModel
        {
            CompanyRoles = companyRoles
        };
        ViewData["CompanySlug"] = slug;
        return View(errorViewModel);
    }

    // GET: /{slug}/staff (placeholder for staff list)
    [HttpGet]
    public IActionResult Index(string slug)
    {
        ViewData["CompanySlug"] = slug;
        return View();
    }
}
