using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class CreateOrderCommandValidator : IValidator<Commands.CreateOrderCommand>
{
    public ValidationResult Validate(Commands.CreateOrderCommand command)
    {
        if (command.CustomerId != Guid.Empty)
            return ValidationResult.Success();

        return ValidationResult.Failure(
            [new ValidationError("customer_id.required", "Customer id is required.")]);
    }
}
