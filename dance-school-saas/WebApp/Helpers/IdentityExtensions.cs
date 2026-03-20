using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Helpers;

public static class IdentityExtensions
{
    public static TKey UserId<TKey>(this ClaimsPrincipal user)
        where TKey : struct
    {
        var claim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        {
            throw new InvalidOperationException("User identity claim not found");
        }

        var stringId = claim.Value.Trim();
        
        try
        {
            if (typeof(TKey) == typeof(Guid))
            {
                return (TKey) Convert.ChangeType(Guid.Parse(stringId), typeof(TKey));
            }
            
            return (TKey) Convert.ChangeType(stringId, typeof(TKey));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert user ID '{stringId}' to type '{typeof(TKey)}'", ex);
        }
    }

    public static Guid UserId(this ClaimsPrincipal user)
    {
        return user.UserId<Guid>();
    }

    private static readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    
    public static string GenerateJwt(
        IEnumerable<Claim> claims,
        string key,
        string issuer,
        string audience,
        DateTime expires)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        return _jwtSecurityTokenHandler.WriteToken(token);
    }
    
    public static bool ValidateJwt(string jwt, string key, string issuer, string audience, bool validateLifetime = true)
    {
        var validationParams = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidateIssuer = true,
            ValidAudience = audience,
            ValidateAudience = true,
            ValidateLifetime = validateLifetime
        };

        try
        {
            _jwtSecurityTokenHandler.ValidateToken(jwt, validationParams, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}