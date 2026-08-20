using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class UserGenreWeightConfiguration : IEntityTypeConfiguration<UserGenreWeight>
{
    public void Configure(EntityTypeBuilder<UserGenreWeight> builder)
    {
        builder.HasKey(ugw => new { ugw.UserId, ugw.GenreId });
        
    }
}