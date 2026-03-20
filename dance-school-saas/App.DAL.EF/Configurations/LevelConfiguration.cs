using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.HasIndex(l => l.CompanyId);
        
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(l => l.Details)
            .HasMaxLength(500);
            
        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}