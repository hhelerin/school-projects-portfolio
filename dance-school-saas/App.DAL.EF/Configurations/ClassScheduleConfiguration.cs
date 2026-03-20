using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
{
    public void Configure(EntityTypeBuilder<ClassSchedule> builder)
    {
        builder.HasIndex(cs => cs.CompanyId);
        builder.HasIndex(cs => cs.ClassId);
        builder.HasIndex(cs => new { cs.StudioRoomId, cs.Date });
        
        builder.Property(cs => cs.Date)
            .IsRequired();
            
        builder.Property(cs => cs.StartTime)
            .IsRequired();
            
        builder.Property(cs => cs.EndTime)
            .IsRequired();
            
        builder.Property(cs => cs.CancellationReason)
            .HasMaxLength(500);
            
        builder.Property(cs => cs.Details)
            .HasMaxLength(1000);
            
        // Configure relationships
        builder.HasOne(cs => cs.Class)
            .WithMany(c => c.Schedules)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(cs => cs.StudioRoom)
            .WithMany()
            .HasForeignKey(cs => cs.StudioRoomId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(cs => !cs.IsDeleted);
    }
}
