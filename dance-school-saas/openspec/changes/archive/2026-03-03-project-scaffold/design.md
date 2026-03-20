## Context

This design builds on the proposal to scaffold the Dance School Platform foundation. The existing WebApp2025 solution provides the project structure, and we're mapping the Clean Architecture layers to the existing projects without creating new ones.

Current state: Basic .NET projects exist with minimal setup. We need to add domain entities, multi-tenancy infrastructure, EF Core configuration, and basic MVC structure.

Constraints:
- Work within existing project structure
- Keep it simple for a practice project
- Use SQLite for database
- No production deployment considerations

Stakeholders: Developers implementing the dance school SaaS platform

## Goals / Non-Goals

**Goals:**
- Establish multi-tenant architecture with path-based routing
- Implement domain entities with proper base classes and soft deletes
- Configure EF Core with Identity and global query filters
- Set up CQRS pattern foundation with MediatR
- Create basic MVC controllers and views
- Add testing framework setup

**Non-Goals:**
- Implement specific dance school business features
- Add REST API endpoints
- Configure production deployment
- Implement advanced security features

## Decisions

1. **Architecture Mapping**: Map Clean Architecture layers to existing projects:
   - App.Domain → Domain layer
   - App.DAL.EF → Infrastructure layer
   - App.BLL → Application layer
   - WebApp → Presentation layer
   Why: Maintains existing structure while following best practices

2. **Database Choice**: SQLite with "Data Source=danceschool.db"
   Why: Simple setup, no external dependencies, suitable for practice project
   Alternative: SQL Server - rejected for complexity

3. **Multi-Tenancy Approach**: Path-based routing with middleware
   Why: Simple to implement, clear URL structure (e.g., /school-name/...)
   Alternative: Subdomain routing - more complex DNS setup

4. **Soft Deletes**: Global query filters exclude IsDeleted = true
   Why: Maintains data integrity, easy to implement
   Alternative: Hard deletes - loses audit trail

5. **Error Handling**: Result<T> pattern in application layer
   Why: Clean separation of business errors from exceptions
   Alternative: Exceptions throughout - harder to handle in controllers

6. **DateTime Handling**: Store all as UTC with global converter
   Why: Consistent timezone handling
   Alternative: Local time - timezone confusion

## Risks / Trade-offs

- **SQLite Limitations**: Single database file may not scale for many tenants
  Mitigation: Use global query filters for data isolation

- **Middleware Complexity**: Tenant resolution middleware adds request overhead
  Mitigation: Keep middleware simple, cache tenant data if needed

- **Global Filters Performance**: EF Core global filters may impact query performance
  Mitigation: Monitor and optimize if issues arise

- **No Rollback Strategy**: Simple scaffold, no complex deployment
  Mitigation: Database migrations can be reverted if needed

## Migration Plan

1. Add NuGet packages to respective projects
2. Update App.Domain with base entities and interfaces
3. Implement App.Helpers with Result<T> and middleware
4. Configure App.DAL.EF with EF Core and Identity
5. Update WebApp Program.cs with DI and middleware
6. Create basic controllers and views
7. Run EF migrations to create database
8. Update test project setup

Rollback: Delete added files, revert NuGet changes, drop database

## Open Questions

- Should we add logging infrastructure?
- How to handle tenant-specific configuration beyond CompanySettings?
- Need for background job infrastructure?