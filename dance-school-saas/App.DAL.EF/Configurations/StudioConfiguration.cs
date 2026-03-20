using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class StudioConfiguration : IEntityTypeConfiguration<Studio>
{
    public void Configure(EntityTypeBuilder<Studio> builder)
    {
        builder.HasIndex(s => s.CompanyId);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.Details)
            .HasMaxLength(500);
            
        builder.Property(s => s.ContactInfo)
            .HasMaxLength(500);
            
        builder.HasMany(s => s.Rooms)
            .WithOne(r => r.Studio)
            .HasForeignKey(r => r.StudioId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}