using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class DanceStyleConfiguration : IEntityTypeConfiguration<DanceStyle>
{
    public void Configure(EntityTypeBuilder<DanceStyle> builder)
    {
        builder.HasIndex(ds => ds.CompanyId);
        
        builder.Property(ds => ds.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(ds => ds.Details)
            .HasMaxLength(500);
            
        builder.HasQueryFilter(ds => !ds.IsDeleted);
    }
}