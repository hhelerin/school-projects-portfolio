using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Helpers;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            await AttachUserToContextAsync(context, token);
        }

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, string token)
    {
        try
        {
            var jwtService = context.RequestServices.GetRequiredService<JwtService>();
            var principal = jwtService.ValidateToken(token);

            if (principal != null)
            {
                context.User = principal;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validating JWT token");
        }
    }
}