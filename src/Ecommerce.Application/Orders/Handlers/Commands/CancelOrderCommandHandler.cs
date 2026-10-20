using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.Exceptions;
using Ecommerce.Application.Orders.DTOs;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class CancelOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<CancelOrderCommand> validator)
    : ICommandHandler<CancelOrderCommand, OrderDto>
{
    public async Task<OrderDto> HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(command).ThrowIfInvalid();

        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.Cancel(command.Motive);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
