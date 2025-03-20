namespace ErrorHandling;

public class ErrorException : Exception
{
    public ErrorException(Error error)
        : base(error.Message)
    {
        Error = error;
    }

    public Error Error { get; }

    public override string ToString() => $"{Error.Code}: {Error.Message}";
}