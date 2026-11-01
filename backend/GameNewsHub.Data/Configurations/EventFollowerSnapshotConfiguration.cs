using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsHub.Data.Configurations;

public class EventFollowerSnapshotConfiguration : IEntityTypeConfiguration<EventFollowerSnapshot>
{
    public void Configure(EntityTypeBuilder<EventFollowerSnapshot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // indeks pod zapytania czasowe / event
        builder.HasIndex(x => new { x.EventId, x.RecordedAt });
    }
}