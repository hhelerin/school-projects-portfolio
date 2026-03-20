using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Domain.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.Helpers;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(AppUser user, IEnumerable<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT:Key is not configured"));
        var issuer = jwtSettings["Issuer"] ?? "DanceSchoolSaaS";
        var audience = jwtSettings["Audience"] ?? "DanceSchoolSaaS";
        var expiresInSeconds = int.TryParse(jwtSettings["ExpiresInSeconds"], out var expSeconds) ? expSeconds : 900;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim("companyId", user.PreferredCompanyId?.ToString() ?? ""),
        };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(expiresInSeconds),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public DateTime GetRefreshTokenExpiry()
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var refreshTokenExpiresInSeconds = int.TryParse(jwtSettings["RefreshTokenExpiresInSeconds"], out var expSeconds)
            ? expSeconds
            : 1209600; // Default: 14 days
        return DateTime.UtcNow.AddSeconds(refreshTokenExpiresInSeconds);
    }
}