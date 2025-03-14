namespace Core;

public class Result<TValue, TError>
{
    private Result(TValue value)
    {
        IsSuccess = true;
        Value = value;
        Error = default;
    }

    private Result(TError error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public TValue? Value { get; }
    public TError? Error { get; }
    public bool IsSuccess { get; }

    public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value);

    public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(error);

    public Result<TValue, TError> Match(Func<TValue, Result<TValue, TError>> success, Func<TError, Result<TValue, TError>> failure)
        => IsSuccess ? success(Value!) : failure(Error!);

    public override string ToString() => $"Value: {Value} - Error: {Error}";
}