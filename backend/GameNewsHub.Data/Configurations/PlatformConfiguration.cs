using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsHub.Data.Configurations;

public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
{
    public void Configure(EntityTypeBuilder<Platform> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.IgdbId).IsUnique();
        builder.HasIndex(x => x.Name);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(x => x.PlatformGroup)
            .WithMany(x => x.Platforms)
            .HasForeignKey(x => x.PlatformGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}