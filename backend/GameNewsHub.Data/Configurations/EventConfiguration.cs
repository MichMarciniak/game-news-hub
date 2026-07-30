using GameNewsHub.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.StartTime);

        builder.HasIndex(x => x.IgdbId).IsUnique();

        builder.Property(x => x.Description)
            .HasColumnType("text");

        builder.HasMany(x => x.Games)
            .WithMany() // nie ma relacji w Game
            .UsingEntity("GameEvents");

        builder.HasMany(e => e.GenreWeights)
            .WithOne();

        builder.Property(e => e.Status)
            .HasConversion<string>();
    }
}