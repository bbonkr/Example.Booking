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

public class BatchUpdateAvailableTimeTableModel : UpdateAvailableTimeTableModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// 0: Sunday ~ 6: Saturday
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }
}

public class BatchAvailableTimeTableModel
{
    public IEnumerable<AddAvailableTimeTableModel> Add { get; set; } = Enumerable.Empty<AddAvailableTimeTableModel>();

    public IEnumerable<BatchUpdateAvailableTimeTableModel> Update { get; set; } = Enumerable.Empty<BatchUpdateAvailableTimeTableModel>();

    public IEnumerable<Guid> Delete { get; set; } = Enumerable.Empty<Guid>();
}

public class BatchAvailableTimeTableResultModel
{
    public IList<AvailableTimeTableModel> Added { get; set; } = new List<AvailableTimeTableModel>();

    public IList<AvailableTimeTableModel> Updated { get; set; } = new List<AvailableTimeTableModel>();

    public IList<Guid> Deleted { get; set; } = new List<Guid>();
}