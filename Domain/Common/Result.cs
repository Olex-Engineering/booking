namespace Booking.Domain.Common;

public abstract record Result<T>
{
  private Result() { }

  public sealed record Ok(T Value) : Result<T>;

  public sealed record Conflict : Result<T>;

  public sealed record Error : Result<T>;
}
