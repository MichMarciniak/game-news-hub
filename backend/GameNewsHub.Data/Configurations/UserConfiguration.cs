using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsHub.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.HasMany(x => x.FollowedPlatformGroups)
            .WithMany()
            .UsingEntity("UserPlatformGroups");

        builder.HasMany(x => x.FollowedGames)
            .WithMany()
            .UsingEntity("UserGames");

        builder.HasMany(x => x.FollowedGenres)
            .WithMany()
            .UsingEntity("UserGenres");

        builder.HasMany(x => x.FollowedEvents)
            .WithMany()
            .UsingEntity("UserEvents");
    }
}