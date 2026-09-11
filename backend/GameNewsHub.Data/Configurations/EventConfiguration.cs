using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsHub.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.StartTime);

        builder.HasIndex(x => x.IgdbId).IsUnique();

        builder.HasIndex(x => x.EventSeriesId);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnType("text");

        builder.HasMany(x => x.Games)
            .WithMany() // nie ma relacji w Game
            .UsingEntity("GameEvents");

        builder.HasMany(e => e.GenreWeights)
            .WithOne();

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.Property(x => x.NormalizedName).HasMaxLength(100);

        builder.HasOne(e => e.Series)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.EventSeriesId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}