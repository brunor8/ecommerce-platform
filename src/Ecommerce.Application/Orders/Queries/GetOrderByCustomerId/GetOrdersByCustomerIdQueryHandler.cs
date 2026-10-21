using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.DTOs;
using Ecommerce.Application.Orders.Queries;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class GetOrdersByCustomerIdQueryHandler(
    IOrderRepository orderRepository,
    IValidator<GetOrdersByCustomerIdQuery> validator)
    : IQueryHandler<GetOrdersByCustomerIdQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> HandleAsync(
        GetOrdersByCustomerIdQuery query,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(query).ThrowIfInvalid();

        var orders = await orderRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
        return orders.Select(OrderDto.FromDomain).ToArray();
    }
}
