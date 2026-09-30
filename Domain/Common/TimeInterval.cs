using System.Text.Json.Serialization;

namespace Booking.Domain.Common;
public readonly record struct TimeInterval
{
  public DateTimeOffset From { get; }
  public DateTimeOffset To { get; }

  public TimeSpan Interval => To - From;


  [JsonConstructor]
  public TimeInterval(DateTimeOffset from, DateTimeOffset to)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(DateTimeOffset.UtcNow, from);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

    From = from;
    To = to;
  }

  public bool IsConflictedWith(TimeInterval interval) => 
    !(To <= interval.From || From >= interval.To);
}
