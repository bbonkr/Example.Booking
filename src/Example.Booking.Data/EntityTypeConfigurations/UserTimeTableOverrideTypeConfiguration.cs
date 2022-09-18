using Example.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Booking.Data.EntityTypeConfigurations;

public class UserTimeTableOverrideTypeConfiguration : IEntityTypeConfiguration<UserTimeTableOverride>
{
    public void Configure(EntityTypeBuilder<UserTimeTableOverride> builder)
    {
        builder.ToTable(nameof(UserTimeTableOverride));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(36)
            ;
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(36)
            ;
        builder.Property(x => x.Date)
            .IsRequired()
            ;
        builder.Property(x => x.Start)
            .IsRequired()
            .HasMaxLength(5)
            ;
        builder.Property(x => x.End)
            .IsRequired()
            .HasMaxLength(5)
            ;

        builder.HasOne(x => x.User)
            .WithMany(x => x.DateTimeOverrides)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            ;
    }
}