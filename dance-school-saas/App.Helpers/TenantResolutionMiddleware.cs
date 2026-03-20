using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Helpers;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, IServiceProvider serviceProvider)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path) || path == "/")
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for API routes
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for Identity pages (login, register, etc.)
        if (path.StartsWith("/Identity", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for account management pages
        if (path.StartsWith("/Account", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            await _next(context);
            return;
        }

        var slug = segments[0];

        // Check if user is authenticated for company context resolution
        var user = context.User;
        var userId = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (user?.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(userId))
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Try to resolve company from URL slug
            var company = await dbContext.Companies
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (company != null)
            {
                // Verify user has access to this company
                if (Guid.TryParse(userId, out var userGuid))
                {
                    var hasAccess = await dbContext.CompanyUsers
                        .AnyAsync(cu => cu.AppUserId == userGuid &&
                                        cu.CompanyId == company.Id &&
                                        cu.IsActive);

                    if (hasAccess)
                    {
                        tenantContext.CompanyId = company.Id;
                        tenantContext.Slug = company.Slug;
                        await _next(context);
                        return;
                    }
                }
            }

            // If user has preferred company set but no company in URL, redirect to preferred
            if (Guid.TryParse(userId, out var parsedUserId))
            {
                var appUser = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == parsedUserId);

                if (appUser?.PreferredCompanyId != null)
                {
                    var preferredCompany = await dbContext.Companies
                        .FirstOrDefaultAsync(c => c.Id == appUser.PreferredCompanyId && c.IsActive);

                    if (preferredCompany != null)
                    {
                        tenantContext.CompanyId = preferredCompany.Id;
                        tenantContext.Slug = preferredCompany.Slug;
                        await _next(context);
                        return;
                    }
                }
            }
        }

        // Fallback to slug-based resolution
        tenantContext.CompanyId = Guid.Empty; // System context
        tenantContext.Slug = slug;
        await _next(context);
    }
}