using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasIndex(i => i.CompanyId);
        builder.HasIndex(i => i.AppUserId);
        
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(i => i.PersonalId)
            .HasMaxLength(50);
            
        builder.Property(i => i.ContactInfo)
            .HasMaxLength(500);
            
        builder.Property(i => i.Details)
            .HasMaxLength(1000);
            
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}