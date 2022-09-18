using Example.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Booking.Data.EntityTypeConfigurations;

public class UserAvailableTimetableTypeConfiguration : IEntityTypeConfiguration<UserAvailableTimetable>
{
    public void Configure(EntityTypeBuilder<UserAvailableTimetable> builder)
    {
        builder.ToTable(nameof(UserAvailableTimetable));

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
        builder.Property(x => x.DayOfWeek)
            .IsRequired()
            .HasConversion<int>()
            ;
        builder.Property(x => x.Start)
            .IsRequired()
            .HasMaxLength(5)
            ;
        builder.Property(x => x.End)
            .IsRequired()
            .HasMaxLength(5);

        builder.HasOne(x => x.User)
            .WithMany(x => x.AvailableTimeTables)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            ;

    }
}
