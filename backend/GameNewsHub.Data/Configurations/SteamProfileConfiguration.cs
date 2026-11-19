using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsHub.Data.Configurations;

public class SteamProfileConfiguration : IEntityTypeConfiguration<SteamProfile>
{
    public void Configure(EntityTypeBuilder<SteamProfile> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.SteamId).IsUnique();

        builder.Property(x => x.LastImportedAt).IsRequired();

        builder.HasOne(x => x.AppUser)
            .WithOne(au => au.SteamProfile)
            .HasForeignKey<SteamProfile>(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}