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

        builder.Property(x => x.FriendSteamIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(ulong.Parse)
                    .ToList());

        builder.HasOne(x => x.AppUser)
            .WithOne(au => au.SteamProfile)
            .HasForeignKey<SteamProfile>(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}