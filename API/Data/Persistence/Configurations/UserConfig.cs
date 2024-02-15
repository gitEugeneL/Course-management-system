using API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Persistence.Configurations;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(u => u.PwdHash)
            .IsRequired();

        builder.Property(u => u.PwdSalt)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired();

        builder.Property(u => u.UniversityNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        /*** Relations ***/
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId);
    }
}