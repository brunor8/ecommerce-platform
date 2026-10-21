using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class GetOrderByIdQueryValidator : IValidator<Queries.GetOrderByIdQuery>
{
    public ValidationResult Validate(Queries.GetOrderByIdQuery query) =>
        query.OrderId == Guid.Empty
            ? ValidationResult.Failure(
                [new ValidationError("order_id.required", "Order id is required.")])
            : ValidationResult.Success();
}
