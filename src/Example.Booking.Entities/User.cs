namespace Example.Booking.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? BeforeEventBuffer { get; set; } = null;
    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? AfterEventBuffer { get; set; } = null;

    public virtual ICollection<UserAvailableTimetable> AvailableTimeTables { get; set; } = new List<UserAvailableTimetable>();

    public virtual ICollection<UserTimeTableOverride> DateTimeOverrides { get; set; } = new List<UserTimeTableOverride>();

    public virtual ICollection<Appointment> RequestedAppointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> ApprovedAppointments { get; set; } = new List<Appointment>();

}
