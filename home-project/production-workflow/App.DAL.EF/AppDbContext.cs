using App.Domain;
using App.Domain.Identity;
using Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Customer> Customers { get; set; }  = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderItem> OrderItems { get; set; } = default!;
    public DbSet<Material> Materials { get; set; } = default!;
    public DbSet<ProcessingStep> ProcessingSteps { get; set; } = default!;
    public DbSet<OperationMapping>  OperationMappings { get; set; } = default!;
    public DbSet<Shipment> Shipments { get; set; } = default!;
    public DbSet<CustomersUsers> CustomersUsers { get; set; } = default!;
    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // remove cascade delete
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Store classifier type enum string representation
        builder
            .Entity<Order>()
            .Property(e => e.Status)
            .HasConversion<string>();

        // We have custom UserRole - with separate PK and navigation for Role and User
        // override default Identity EF config
        builder.Entity<AppUserRole>().HasKey(a => a.Id);

        builder.Entity<AppUserRole>().HasIndex(a => new {a.UserId, a.RoleId}).IsUnique();

        builder.Entity<AppUserRole>()
            .HasOne(a => a.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(a => a.UserId);

        builder.Entity<AppUserRole>()
            .HasOne(a => a.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(a => a.RoleId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var addedEntries = ChangeTracker.Entries()
            .Where(e => e is { Entity: IDomainMeta });
        foreach (var entry in addedEntries)
        {
            if (entry.State == EntityState.Added)
            {
                (entry.Entity as IDomainMeta)!.CreatedAt = DateTime.UtcNow;
                (entry.Entity as IDomainMeta)!.CreatedBy = "system";
            }
            else if (entry.State == EntityState.Modified)
            {
                (entry.Entity as IDomainMeta)!.ModifiedAt = DateTime.UtcNow;
                (entry.Entity as IDomainMeta)!.ModifiedBy = "system";
                
                // Prevent overwriting CreatedBy/CreatedAt/UserId on update
                entry.Property("CreatedAt").IsModified = false;
                entry.Property("CreatedBy").IsModified = false;

                entry.Property("UserId").IsModified = false;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
