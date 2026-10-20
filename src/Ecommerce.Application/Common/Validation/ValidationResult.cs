namespace Ecommerce.Application.Common.Validation;

public sealed class ValidationResult
{
    public IReadOnlyList<ValidationError> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    private ValidationResult(IReadOnlyList<ValidationError> errors)
    {
        Errors = errors;
    }

    public static ValidationResult Success() => new(Array.Empty<ValidationError>());

    public static ValidationResult Failure(IEnumerable<ValidationError> errors) =>
        new(errors.ToArray());

    public void ThrowIfInvalid()
    {
        if (!IsValid)
            throw new ValidationException(Errors);
    }
}
