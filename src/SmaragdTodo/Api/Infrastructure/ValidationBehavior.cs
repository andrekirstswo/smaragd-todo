using FluentValidation;
using MediatR;
using Api.Exceptions;

namespace Api.Infrastructure;

/// <summary>
/// ValidationBehavior is a MediatR pipeline behavior that performs validation on the incoming request
/// using FluentValidation validators. If any validation errors are found, a ValidationException is thrown.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the validation of the incoming request using FluentValidation validators.
    /// If validation errors are found, a ValidationException is thrown.
    /// Otherwise, the request is passed to the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The incoming request to be validated.</param>
    /// <param name="next">The next handler in the MediatR pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation errors are found.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            throw new Exceptions.ValidationException(errors);
        }

        return await next();
    }
}
