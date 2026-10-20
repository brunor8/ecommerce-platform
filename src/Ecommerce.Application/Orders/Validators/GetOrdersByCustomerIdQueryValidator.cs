using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Orders.Validators;

public sealed class GetOrdersByCustomerIdQueryValidator : IValidator<Queries.GetOrdersByCustomerIdQuery>
{
    public ValidationResult Validate(Queries.GetOrdersByCustomerIdQuery query) =>
        query.CustomerId == Guid.Empty
            ? ValidationResult.Failure(
                [new ValidationError("customer_id.required", "Customer id is required.")])
            : ValidationResult.Success();
}
