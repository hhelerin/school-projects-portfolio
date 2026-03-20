using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Setup;

public static class IdentitySetupExtensions
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                // Sign-in settings
                options.SignIn.RequireConfirmedAccount = false;

                // Password complexity requirements (Task 9.1)
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 4;

                // Account lockout policy (Task 9.2)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Token lifespan (for password reset, email confirmation, etc.)
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Configure application cookies for session management (Task 8.2, 8.5)
        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allow HTTP in dev
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Name = ".App.Auth";

            // Session timeout (Task 8.5)
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;

            // Login/logout paths
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // System roles policies
            options.AddPolicy("RequireSystemAdmin", policy =>
                policy.RequireRole(CompanyRoleType.SystemAdmin.ToString()));

            options.AddPolicy("RequireSystemSupport", policy =>
                policy.RequireRole(
                    CompanyRoleType.SystemAdmin.ToString(),
                    CompanyRoleType.SystemSupport.ToString()));

            options.AddPolicy("RequireSystemBilling", policy =>
                policy.RequireRole(
                    CompanyRoleType.SystemAdmin.ToString(),
                    CompanyRoleType.SystemBilling.ToString()));

            // Company management policies
            options.AddPolicy("RequireCompanyOwner", policy =>
                policy.RequireRole(CompanyRoleType.CompanyOwner.ToString()));

            options.AddPolicy("RequireCompanyAdmin", policy =>
                policy.RequireRole(
                    CompanyRoleType.CompanyOwner.ToString(),
                    CompanyRoleType.CompanyAdmin.ToString()));

            options.AddPolicy("RequireCompanyManager", policy =>
                policy.RequireRole(
                    CompanyRoleType.CompanyOwner.ToString(),
                    CompanyRoleType.CompanyAdmin.ToString(),
                    CompanyRoleType.CompanyManager.ToString()));

            // All authenticated company users
            options.AddPolicy("RequireCompanyEmployee", policy =>
                policy.RequireRole(
                    CompanyRoleType.CompanyOwner.ToString(),
                    CompanyRoleType.CompanyAdmin.ToString(),
                    CompanyRoleType.CompanyManager.ToString(),
                    CompanyRoleType.CompanyEmployee.ToString()));

            // Any system role
            options.AddPolicy("RequireSystemRole", policy =>
                policy.RequireRole(
                    CompanyRoleType.SystemAdmin.ToString(),
                    CompanyRoleType.SystemSupport.ToString(),
                    CompanyRoleType.SystemBilling.ToString()));
        });

        return services;
    }
}