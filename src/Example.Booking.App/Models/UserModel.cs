using System.ComponentModel.DataAnnotations;
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

public class AddUserModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? BeforeEventBuffer { get; set; } = null;
    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? AfterEventBuffer { get; set; } = null;
}

public class UpdateUserModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? BeforeEventBuffer { get; set; } = null;
    /// <summary>
    /// Value should be minutes
    /// </summary>
    public int? AfterEventBuffer { get; set; } = null;
}