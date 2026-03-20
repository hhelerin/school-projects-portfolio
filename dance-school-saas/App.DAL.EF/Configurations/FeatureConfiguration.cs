using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(f => f.Details)
            .HasMaxLength(500);
            
        builder.HasData(
            new Feature { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Sprung Floor", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Feature { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Mirrors", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Feature { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Barres", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Feature { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Poles", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Feature { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Aerial Rigging", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Feature { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Sound System", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}