using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class ApproveOrderCommandValidator : IValidator<Commands.ApproveOrderCommand>
{
    public ValidationResult Validate(Commands.ApproveOrderCommand command) =>
        command.OrderId == Guid.Empty
            ? ValidationResult.Failure(
                [new ValidationError("order_id.required", "Order id is required.")])
            : ValidationResult.Success();
}
