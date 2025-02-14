namespace Api.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationError>? Errors { get; }

    public ValidationException(IEnumerable<ValidationError>? errors)
    {
        Errors = errors;
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);