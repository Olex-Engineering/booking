namespace Booking.Domain.Common;

public sealed record Result<T>
{
  private readonly T? _value;

  public ResultType Type { get; }

  public string? Message { get; }

  public T Value => Type == ResultType.Ok
    ? _value!
    : throw new InvalidOperationException($"Cannot read Value of a {Type} result");

  private bool PrintMembers(System.Text.StringBuilder builder)
  {
    builder.Append($"Type = {Type}");
    if (Type == ResultType.Ok) builder.Append($", Value = {_value}");
    if (Message is not null) builder.Append($", Message = {Message}");
    return true;
  }

  private Result(ResultType type, T? value = default, string? message = null)
  {
    Type = type;
    _value = value;
    Message = message;
  }

  public static Result<T> Ok(T value)
  {
    return new Result<T>(ResultType.Ok, value);
  }

  public static Result<T> NotFound(string message)
  {
    return new Result<T>(ResultType.NotFound, message: message);
  }

  public static Result<T> Conflict(string message)
  {
    return new Result<T>(ResultType.Conflict, message: message);
  }
}
