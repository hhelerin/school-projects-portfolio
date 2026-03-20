using App.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Helpers.Authorization;

public class CompanyRoleAuthorizationHandler : AuthorizationHandler<CompanyRoleAuthorizationRequirement>
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CompanyRoleAuthorizationHandler(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CompanyRoleAuthorizationRequirement requirement)
    {
        var userId = _currentUserService.UserId;
        var companyId = _currentUserService.CompanyId;

        if (userId == Guid.Empty || companyId == Guid.Empty)
        {
            return;
        }

        // Check if user has any of the required company roles
        var hasRole = await _context.CompanyUserRoles
            .Where(cur => cur.CompanyUser!.AppUserId == userId
                       && cur.CompanyUser.CompanyId == companyId
                       && cur.IsActive)
            .Select(cur => cur.CompanyRole!.Name)
            .AnyAsync(roleName => requirement.AllowedRoles.Contains(roleName));

        if (hasRole)
        {
            context.Succeed(requirement);
            return;
        }

        // Fallback: Check Identity roles if no company user roles found
        // This allows system-level roles to grant access
        var userRoles = _currentUserService.Roles;
        if (userRoles.Any(role => requirement.AllowedRoles.Contains(role)))
        {
            context.Succeed(requirement);
        }
    }
}
