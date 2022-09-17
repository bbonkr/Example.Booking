using System.Text.Json.Serialization;

namespace Example.Booking.App.Models;

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

public class AddAvailableTimeTableModel
{
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

public class UpdateAvailableTimeTableModel
{
    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// Format: HH:mm
    /// </summary>
    public string End { get; set; } = string.Empty;
}

