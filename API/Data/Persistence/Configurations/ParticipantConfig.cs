using API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Persistence.Configurations;

public class ParticipantConfig : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.Property(p => p.Grade)
            .IsRequired();

        builder.Property(p => p.ProfessorNote)
            .IsRequired()
            .HasMaxLength(250);
    }
}