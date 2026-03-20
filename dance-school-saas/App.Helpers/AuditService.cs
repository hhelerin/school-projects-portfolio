using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Http;

namespace App.Helpers;

public interface IAuditService
{
    Task LogSecurityEventAsync(
        string action,
        string entityType,
        Guid entityId,
        string? details = null);
}

public class AuditService : IAuditService
{
    // Consistent GUIDs for system/anonymous contexts
    private static readonly Guid SystemUserId = new Guid("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SystemCompanyId = new Guid("00000000-0000-0000-0000-000000000002");

    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public AuditService(
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }

    public async Task LogSecurityEventAsync(
        string action,
        string entityType,
        Guid entityId,
        string? details = null)
    {
        var userId = _currentUserService.UserId;
        var companyId = _currentUserService.CompanyId;

        // Store IP and UserAgent in NewValues for security context
        var httpContext = _httpContextAccessor.HttpContext;
        var securityContext = $"IP: {httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown"}, Agent: {httpContext?.Request?.Headers["User-Agent"].ToString() ?? "unknown"}, Details: {details ?? "none"}";

        var auditLog = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId == Guid.Empty ? SystemUserId : userId,
            Timestamp = DateTime.UtcNow,
            NewValues = securityContext,
            CompanyId = companyId == Guid.Empty ? SystemCompanyId : companyId
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
