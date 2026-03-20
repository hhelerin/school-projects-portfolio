using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace App.Helpers;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenantContext;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor, 
        ITenantContext tenantContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantContext = tenantContext;
    }

    public Guid UserId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return Guid.Empty;
            
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
        }
    }

    public Guid CompanyId => _tenantContext.CompanyId;

    public Guid? GetCurrentUserId()
    {
        var userId = UserId;
        return userId == Guid.Empty ? null : userId;
    }

    public string[] Roles
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return Array.Empty<string>();
            
            return context.User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();
        }
    }
}