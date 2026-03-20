using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.HasIndex(c => c.CompanyId);
        builder.HasIndex(c => c.StudioRoomId);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Details)
            .HasMaxLength(1000);
            
        builder.Property(c => c.Capacity)
            .IsRequired();
            
        // Configure RecurrencePattern as owned entity
        builder.OwnsOne(c => c.Recurrence, recurrence =>
        {
            recurrence.Property(r => r.DayOfWeek)
                .IsRequired();
                
            recurrence.Property(r => r.StartTime)
                .IsRequired();
                
            recurrence.Property(r => r.EndTime)
                .IsRequired();
                
            recurrence.Property(r => r.RecurrenceStartDate)
                .IsRequired();
                
            recurrence.Property(r => r.RecurrenceEndDate)
                .IsRequired();
        });
        
        // Configure relationships
        builder.HasOne(c => c.StudioRoom)
            .WithMany()
            .HasForeignKey(c => c.StudioRoomId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Instructor)
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.DanceStyle)
            .WithMany()
            .HasForeignKey(c => c.DanceStyleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Level)
            .WithMany()
            .HasForeignKey(c => c.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
