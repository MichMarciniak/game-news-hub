using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.HasMany(x => x.Interests)
            .WithOne(e => e.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Platforms)
            .WithMany()
            .UsingEntity("UserPlatforms");

        builder.HasMany(x => x.FollowedGames)
            .WithMany()
            .UsingEntity("UserGames");
    }
}