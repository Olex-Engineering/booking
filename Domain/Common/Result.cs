namespace Booking.Domain.Common;

public sealed record Result<T>
{
public ResultType Type { get; }
public T? Value { get; }


  private Result(ResultType type, T? value)
  {
    Type = type;
    Value = value;
  }

  private Result(ResultType type)
  {
    Type = type;
  }

  public static Result<T> Ok(T value)
  {
    return new Result<T>(ResultType.Ok, value);
  }

  public static Result<T?> Error()
  {
    return new Result<T?>(ResultType.Error);
  }

  public static Result<T?> Conflict()
  {
    return new Result<T?>(ResultType.Conflict);
  }
};
