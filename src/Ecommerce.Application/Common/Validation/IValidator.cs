namespace Ecommerce.Application.Common.Validation;

public interface IValidator<in TInput>
{
    ValidationResult Validate(TInput input);
}
