using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class EventGenreWeightConfiguration : IEntityTypeConfiguration<EventGenreWeight>
{
    public void Configure(EntityTypeBuilder<EventGenreWeight> builder)
    {
        builder.HasKey(egw => new { egw.EventId, egw.GenreId });

        builder.HasOne<Event>()
            .WithMany(e => e.GenreWeights)
            .HasForeignKey(egw => egw.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Genre>()
            .WithMany()
            .HasForeignKey(egw => egw.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(egw => egw.Weight)
            .IsRequired();
    }
}