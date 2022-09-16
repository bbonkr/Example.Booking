using Example.Booking.Entities;

namespace Example.Booking.App.Models;

public class UserModel
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

    public virtual IEnumerable<AvailableTimeTableModel> AvailableTimeTables { get; set; } = new List<AvailableTimeTableModel>();

    public virtual IEnumerable<TimeTableOverrideModel> DateTimeOverrides { get; set; } = new List<TimeTableOverrideModel>();

    public virtual IEnumerable<AppointmentModel> RequestedAppointments { get; set; } = new List<AppointmentModel>();

    public virtual IEnumerable<AppointmentModel> ApprovedAppointments { get; set; } = new List<AppointmentModel>();
}

public class AvailableTimeTableModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// 0: Sunday ~ 6: Saturday
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string End { get; set; } = string.Empty;
}

public class TimeTableOverrideModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// Date only
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string End { get; set; } = string.Empty;
}

public class AppointmentModel
{
    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    /// <summary>
    /// User who request appointment
    /// </summary>
    public virtual UserModel FromUser { get; set; }

    public virtual UserModel ToUser { get; set; }
}