using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class CancelOrderCommandValidator : IValidator<Commands.CancelOrderCommand>
{
    public ValidationResult Validate(Commands.CancelOrderCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.OrderId == Guid.Empty)
            errors.Add(new ValidationError("order_id.required", "Order id is required."));

        if (string.IsNullOrWhiteSpace(command.Motive))
            errors.Add(new ValidationError("motive.required", "A reason for cancellation is required."));
        else if (command.Motive.Length > 200)
            errors.Add(new ValidationError("motive.max_length", "The reason cannot exceed 200 characters."));

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}
