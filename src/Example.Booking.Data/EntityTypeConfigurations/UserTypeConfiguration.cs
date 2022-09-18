using System;
using Example.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Booking.Data.EntityTypeConfigurations;

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(36)
            ;

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            ;

        builder.Property(x => x.BeforeEventBuffer)
            .IsRequired(false)
            ;

        builder.Property(x => x.AfterEventBuffer)
            .IsRequired(false)
            ;
    }
}
