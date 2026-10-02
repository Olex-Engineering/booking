namespace Booking.Domain.Common;

public sealed record Result<T>
{
  private readonly T? _value;

  public ResultType Type { get; }

  public T Value => Type == ResultType.Ok
    ? _value!
    : throw new InvalidOperationException($"Cannot read Value of a {Type} result");

  public bool IsOk => Type == ResultType.Ok;
  public bool IsError => Type == ResultType.Error || Type == ResultType.Conflict;


  private bool PrintMembers(System.Text.StringBuilder builder)
  {
    builder.Append($"Type = {Type}");
    if (Type == ResultType.Ok) builder.Append($", Value = {_value}");
    return true;
  }

  private Result(ResultType type, T? value = default)
  {
    Type = type;
    _value = value;
  }

  public static Result<T> Ok(T value)
  {
    return new Result<T>(ResultType.Ok, value);
  }

  public static Result<T> Error()
  {
    return new Result<T>(ResultType.Error);
  }

  public static Result<T> Conflict()
  {
    return new Result<T>(ResultType.Conflict);
  }
}
