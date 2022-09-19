using System;
namespace Example.Booking.Extensions;

public static class StringExtentions
{
    public static TimeOnly? ToTimeOnly(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value), $"Value can not be null");
        }

        var isValid = value.IsValidTimeOnlyString();
        if (!isValid)
        {
            throw new Exception($"Value is invalid. (value={value})");
        }

        var (hour, minute) = value.ToParseHourAndMinute();

        return new TimeOnly(hour, minute);
    }

    public static bool IsValidTimeOnlyString(this string value)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\d{2}:\d{2}$");

        var isMatched = regex.IsMatch(value);

        if (!isMatched)
        {
            return false;
        }

        try
        {
            var (hour, minite) = value.ToParseHourAndMinute();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsIncluded(this string timeValue, string start, string end)
    {
        var startTime = start.ToTimeOnly();
        var endTime = end.ToTimeOnly();
        if (startTime.HasValue && endTime.HasValue)
        {
            return IsIncluded(timeValue, startTime.Value, endTime.Value);
        }

        return false;
    }

    public static bool IsIncluded(this string timeValue, TimeOnly start, TimeOnly end)
    {
        var current = timeValue.ToTimeOnly();

        if (!current.HasValue)
        {
            return false;
        }

        return start <= current.Value && current.Value <= end;
    }

    private static (int hour, int minute) ToParseHourAndMinute(this string value)
    {
        var tokens = value.Split(':');

        if (tokens.Length != 2)
        {
            throw new Exception("Value must be format with 'HH:mm'");
        }

        if (!int.TryParse(tokens[0], out int hourValue))
        {
            throw new Exception("Hour values must be convertible to integers.");
        }

        if (hourValue < 0 || hourValue >= 24)
        {
            throw new Exception("Hour values must be between 0 and 23");
        }

        if (!int.TryParse(tokens[1], out int minuteValue))
        {
            throw new Exception("Minute values must be convertible to integers.");
        }

        if (minuteValue < 0 || minuteValue >= 60)
        {
            throw new Exception("Minute values must be between 0 and 59");
        }

        return (hourValue, minuteValue);
    }
}

public static class TimeOnlyExtensions
{
    public static bool IsIncluded(this TimeOnly current, string start, string end)
    {
        var tmpStart = start.ToTimeOnly();
        var tmpEnd = end.ToTimeOnly();

        if (tmpStart.HasValue && tmpEnd.HasValue)
        {
            return current.IsIncluded(tmpStart.Value, tmpEnd.Value);
        }

        return false;
    }

    public static bool IsIncluded(this TimeOnly current, TimeOnly start, TimeOnly end)
    {
        return start <= current && current <= end;
    }
}
