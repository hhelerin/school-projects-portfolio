## Why

The Dance School Platform requires a solid foundation to support multi-tenant SaaS operations for dance schools. This scaffold establishes the core architecture, entities, and infrastructure without disrupting the existing WebApp2025 structure.

## What Changes

- App.Domain: Add BaseEntity, ITenantEntity, IRepository<T>, IUnitOfWork, enums for subscription tiers, billing cycles, company roles
- App.Domain: Core entities Company, CompanySettings
- App.Helpers: Result<T>, ITenantContext, ICurrentUserService, TenantResolutionMiddleware
- App.DAL.EF: AppUser extending IdentityUser, CompanyUser, CompanyRole, CompanyUserRole, AuditLog, AppDbContext with global filters, Repository<T>, UnitOfWork, InitialCreate migration
- App.BLL: Empty Features/ folder structure
- App.DTO: Empty, ready for future changes
- WebApp Program.cs: EF Core with SQLite, ASP.NET Core Identity, MediatR, FluentValidation, tenant context, middleware pipeline
- WebApp Views and Controllers: _Layout with Bootstrap, HomeController, Home/Index view
- WebApp.Tests: xUnit and Moq setup
- NuGet packages: Add required packages to respective projects

## Capabilities

### New Capabilities
- multi-tenant-core: Core multi-tenancy infrastructure including tenant resolution, company management, and user roles
- dance-school-entities: Domain entities for dance school operations (companies, users, settings)
- authentication-system: ASP.NET Core Identity integration with custom user and role management
- data-access-layer: EF Core setup with repositories, unit of work, and migrations
- web-foundation: Basic MVC structure with views and controllers

### Modified Capabilities
None - this is the initial scaffold

## Impact

- Updates to all existing projects (App.Domain, App.DAL.EF, App.BLL, App.DTO, App.Helpers, WebApp, WebApp.Tests)
- New database schema with migrations
- New NuGet package references
- No breaking changes to existing code structure