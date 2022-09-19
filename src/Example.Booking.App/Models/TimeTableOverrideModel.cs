namespace Example.Booking.App.Models;

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


public class AddTimetableOverrideModel
{
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

public class UpdateTimetableOverrideModel
{
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

