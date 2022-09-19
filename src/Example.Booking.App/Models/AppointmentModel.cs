namespace Example.Booking.App.Models;

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

public class AddAppintmentModel
{
    public DateTime Date { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    /// <summary>
    /// User who requested
    /// </summary>
    public Guid FromUserId { get; set; }

    /// <summary>
    /// User who makes an appointment with
    /// </summary>
    public Guid ToUserId { get; set; }
}