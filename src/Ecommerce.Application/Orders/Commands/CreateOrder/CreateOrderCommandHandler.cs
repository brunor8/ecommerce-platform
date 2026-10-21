using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.DTOs;
using Ecommerce.Domain.Orders;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateOrderCommand> validator)
    : ICommandHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(command).ThrowIfInvalid();

        var order = Order.Create(command.CustomerId);
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
