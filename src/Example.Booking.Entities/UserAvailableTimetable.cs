namespace Example.Booking.Entities;

public class UserAvailableTimetable
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; }

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
