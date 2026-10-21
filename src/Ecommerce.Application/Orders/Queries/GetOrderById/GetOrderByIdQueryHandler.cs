using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.DTOs;
using Ecommerce.Application.Orders.Exceptions;
using Ecommerce.Application.Orders.Queries;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    IValidator<GetOrderByIdQuery> validator)
    : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> HandleAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(query).ThrowIfInvalid();

        var order = await orderRepository.GetByIdAsync(query.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(query.OrderId);

        return OrderDto.FromDomain(order);
    }
}
