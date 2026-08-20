using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Title);

        builder.HasIndex(x => x.ParentGameId);

        builder.Property(x => x.Summary)
            .HasColumnType("text");

        builder.HasMany(x => x.Genres)
            .WithMany(e => e.Games)
            .UsingEntity("GameGenres");

        builder.HasMany(x => x.Platforms)
            .WithMany(e => e.Games)
            .UsingEntity("GamePlatforms");

        // self reference: gra (DLC/mod/...) -> gra bazowa
        builder.HasOne(x => x.ParentGame)
            .WithMany(x => x.ChildGames)
            .HasForeignKey(x => x.ParentGameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}