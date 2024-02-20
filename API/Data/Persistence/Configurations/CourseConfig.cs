using API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Persistence.Configurations;

public class CourseConfig : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.MaxParticipants)
            .IsRequired();

        builder.Property(c => c.OwnerId)
            .IsRequired();
        
        /*** Relations ***/
        builder.HasMany(c => c.Participants)
            .WithOne(p => p.Course)
            .HasForeignKey(p => p.CourseId);
    }
}