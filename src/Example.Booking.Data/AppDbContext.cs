using Example.Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.Booking.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }

    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<UserAvailableTimetable> UserAvailableTimetables { get; set; }

    public DbSet<UserTimeTableOverride> UserTimeTableOverrides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
