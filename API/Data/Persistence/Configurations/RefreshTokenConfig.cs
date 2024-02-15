using API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Persistence.Configurations;

internal class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.Expires)
            .IsRequired();
    }
}