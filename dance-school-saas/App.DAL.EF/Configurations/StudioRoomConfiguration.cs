using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class StudioRoomConfiguration : IEntityTypeConfiguration<StudioRoom>
{
    public void Configure(EntityTypeBuilder<StudioRoom> builder)
    {
        builder.Property(sr => sr.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(sr => sr.Details)
            .HasMaxLength(500);
            
        builder.HasOne(sr => sr.Studio)
            .WithMany(s => s.Rooms)
            .HasForeignKey(sr => sr.StudioId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(sr => sr.Features)
            .WithOne(sf => sf.StudioRoom)
            .HasForeignKey(sf => sf.StudioRoomId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasQueryFilter(sr => !sr.IsDeleted);
    }
}