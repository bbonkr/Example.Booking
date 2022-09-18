using Example.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Booking.Data.EntityTypeConfigurations;

public class AppointmentTypeConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable(nameof(Appointment));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(36)
            ;
        builder.Property(x => x.FromUserId)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(36)
            ;
        builder.Property(x => x.ToUserId)
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

        builder.HasOne(x => x.FromUser)
            .WithMany(x => x.RequestedAppointments)
            .HasForeignKey(x => x.FromUserId)
            .OnDelete(DeleteBehavior.NoAction)
            ;
        builder.HasOne(x => x.ToUser)
            .WithMany(x => x.ApprovedAppointments)
            .HasForeignKey(x => x.ToUserId)
            .OnDelete(DeleteBehavior.NoAction)
            ;
    }
}