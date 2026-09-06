using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class PlatformGroupConfiguration : IEntityTypeConfiguration<PlatformGroup>
{
    public void Configure(EntityTypeBuilder<PlatformGroup> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        

    }
}