namespace Example.Booking.Entities;

public class Appointment
{
    public Guid Id { get; set; }

    /// <summary>
    /// User who request appointment
    /// </summary>
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public DateTime Date { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    public virtual User FromUser { get; set; }

    public virtual User ToUser { get; set; }
}