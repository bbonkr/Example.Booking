namespace Example.Booking.Entities;

public class UserTimeTableOverride
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

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

    public virtual User User { get; set; }
}
