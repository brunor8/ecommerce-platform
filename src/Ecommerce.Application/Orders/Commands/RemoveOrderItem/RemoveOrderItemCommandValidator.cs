using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class RemoveOrderItemCommandValidator : IValidator<Commands.RemoveOrderItemCommand>
{
    public ValidationResult Validate(Commands.RemoveOrderItemCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.OrderId == Guid.Empty)
            errors.Add(new ValidationError("order_id.required", "Order id is required."));

        if (command.ProductId == Guid.Empty)
            errors.Add(new ValidationError("product_id.required", "Product id is required."));

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}
