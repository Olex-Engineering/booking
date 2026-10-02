namespace Booking.Domain.Common;

public sealed record TimeInterval
{
  public DateTimeOffset From { get; }
  public DateTimeOffset To { get; }


  public TimeInterval(DateTimeOffset from, DateTimeOffset to)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(from, to);

    From = from;
    To = to;
  }

  public bool IsConflictedWith(TimeInterval interval) => 
    !(To <= interval.From || From >= interval.To);
}
