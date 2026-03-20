using System.Text;
using App.BLL.Validators;
using App.DAL.EF;
using App.Domain;
using App.Helpers;
using App.Helpers.Authorization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApp.Helpers;
using WebApp.Setup;

var builder = WebApplication.CreateBuilder(args);

// Service registration
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);
builder.Services.AddAppIdentity();
builder.Services.AddAppAuthorization();

// Register custom company role authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CompanyRoles:CompanyAdmin,CompanyOwner", policy =>
    {
        policy.Requirements.Add(new CompanyRoleAuthorizationRequirement("CompanyAdmin", "CompanyOwner"));
    });
    
    options.AddPolicy("CompanyRoles:CompanyAdmin,CompanyManager", policy =>
    {
        policy.Requirements.Add(new CompanyRoleAuthorizationRequirement("CompanyAdmin", "CompanyManager"));
    });
    
    // Policy that includes CompanyOwner with Admin/Manager roles
    options.AddPolicy("CompanyRoles:CompanyOwner,CompanyAdmin,CompanyManager", policy =>
    {
        policy.Requirements.Add(new CompanyRoleAuthorizationRequirement("CompanyOwner", "CompanyAdmin", "CompanyManager"));
    });
    options.AddPolicy("CompanyRoles:CompanyOwner,CompanyAdmin,CompanyManager,CompanyEmployee", policy =>
    {
        policy.Requirements.Add(new CompanyRoleAuthorizationRequirement("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee"));
    });
});

// Register the authorization handler
builder.Services.AddScoped<IAuthorizationHandler, CompanyRoleAuthorizationHandler>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App.BLL.Commands.SignUpSchoolCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<SignUpSchoolCommandValidator>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAuthentication(options =>
{
    // Set Identity as the default for web requests, JWT for API
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
    };
});

// API Versioning must be added BEFORE controllers
builder.Services.AddAppApiVersioning();

builder.Services.AddAppControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AppNameService>();
builder.Services.AddForwardedHeaders();
builder.Services.AddAppCors();
builder.Services.AddAppSwagger();
builder.Services.AddAppLocalization(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Build and configure pipeline
var app = builder.Build();

app.SetupAppData();
app.UseAppMiddleware();
app.UseAppSwagger();
app.MapAppEndpoints();

app.Run();

// this is needed for unit testing
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program
{
}
