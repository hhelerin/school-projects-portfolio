using App.Helpers;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WebApp.Setup;

public static class MiddlewareExtensions
{
    public static WebApplication UseAppMiddleware(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseRequestLocalization(
            options: app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value!);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Security headers (Task 9.3)
        app.Use(async (context, next) =>
        {
            // Content Security Policy
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; img-src 'self' data: https:; font-src 'self' https://cdn.jsdelivr.net;";

            // X-Content-Type-Options
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // X-Frame-Options
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // X-XSS-Protection
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Permissions-Policy
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

            await next();
        });

        // Only use HTTPS redirection when not in Development or when HTTPS is configured
        if (!app.Environment.IsDevelopment() || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")))
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseSession();

        app.UseAuthentication();

        // Tenant resolution must run AFTER authentication (so User is populated)
        // but BEFORE authorization (so CompanyId is available for auth checks)
        app.UseMiddleware<TenantResolutionMiddleware>();

        app.UseAuthorization();

        return app;
    }

    public static WebApplication UseAppSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant()
                );
            }
            // serve from root
            // options.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapStaticAssets();

        // Redirect /index.html to root (browser may redirect to /index.html from cache/history)
        app.MapGet("/index.html", context =>
        {
            context.Response.Redirect("/", permanent: true);
            return Task.CompletedTask;
        });

        app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages()
            .WithStaticAssets();

        // Explicitly map API controllers
        app.MapControllers();

        return app;
    }
}