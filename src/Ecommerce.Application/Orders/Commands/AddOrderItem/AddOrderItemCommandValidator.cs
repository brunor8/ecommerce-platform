using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class AddOrderItemCommandValidator : IValidator<Commands.AddOrderItemCommand>
{
    public ValidationResult Validate(Commands.AddOrderItemCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.OrderId == Guid.Empty)
            errors.Add(new ValidationError("order_id.required", "Order id is required."));

        if (command.ProductId == Guid.Empty)
            errors.Add(new ValidationError("product_id.required", "Product id is required."));

        if (string.IsNullOrWhiteSpace(command.ProductName))
            errors.Add(new ValidationError("product_name.required", "Product name is required."));

        if (command.UnitPrice < 0)
            errors.Add(new ValidationError("unit_price.negative", "Unit price cannot be negative."));

        if (command.Quantity <= 0)
            errors.Add(new ValidationError("quantity.invalid", "Quantity must be greater than zero."));

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}
