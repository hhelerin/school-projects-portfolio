using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Helpers;

public class RefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public RefreshTokenService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AppRefreshToken> CreateRefreshTokenAsync(AppUser user, string? deviceInfo = null)
    {
        // Revoke any existing refresh tokens for this user (optional - for single device login)
        // await RevokeUserTokensAsync(user.Id);

        var refreshToken = new AppRefreshToken
        {
            AppUserId = user.Id,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            ExpiresAt = _jwtService.GetRefreshTokenExpiry(),
            DeviceInfo = deviceInfo,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<AppRefreshToken?> GetValidRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.AppUser)
            .FirstOrDefaultAsync(rt =>
                rt.RefreshToken == token &&
                !rt.IsRevoked &&
                rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.RefreshToken == token);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeUserTokensAsync(Guid userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.AppUserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<AppRefreshToken> RotateRefreshTokenAsync(string oldToken, AppUser user)
    {
        // Revoke the old token
        await RevokeRefreshTokenAsync(oldToken);

        // Create a new token
        var newToken = new AppRefreshToken
        {
            AppUserId = user.Id,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            ExpiresAt = _jwtService.GetRefreshTokenExpiry(),
            IsRevoked = false,
            PreviousToken = oldToken // Link to old token for audit
        };

        _context.RefreshTokens.Add(newToken);
        await _context.SaveChangesAsync();

        return newToken;
    }

    public async Task CleanExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt <= DateTime.UtcNow && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in expiredTokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }
}