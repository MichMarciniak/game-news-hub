using GameNewsHub.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.HasKey(ui => new { ui.UserId, ui.GenreId });

        builder.HasOne(ui => ui.User)
            .WithMany(u => u.Interests)
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ui => ui.Genre)
            .WithMany()
            .HasForeignKey(ui => ui.GenreId)
            .OnDelete(DeleteBehavior.Cascade); //raczej nie zostanie usunięte i tak
    }
}