using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.AppUserId);
        builder.HasIndex(s => new { s.CompanyId, s.Name });
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.PersonalId)
            .HasMaxLength(50);
            
        builder.Property(s => s.ContactInfo)
            .HasMaxLength(500);
            
        builder.Property(s => s.Details)
            .HasMaxLength(1000);
            
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
