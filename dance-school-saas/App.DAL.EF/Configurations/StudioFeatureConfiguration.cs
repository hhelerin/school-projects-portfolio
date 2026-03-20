using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.EF.Configurations;

public class StudioFeatureConfiguration : IEntityTypeConfiguration<StudioFeature>
{
    public void Configure(EntityTypeBuilder<StudioFeature> builder)
    {
        builder.HasOne(sf => sf.StudioRoom)
            .WithMany(sr => sr.Features)
            .HasForeignKey(sf => sf.StudioRoomId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(sf => sf.Feature)
            .WithMany()
            .HasForeignKey(sf => sf.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Property(sf => sf.Details)
            .HasMaxLength(500);
    }
}