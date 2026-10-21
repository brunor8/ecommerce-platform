using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.DTOs;
using Ecommerce.Application.Orders.Exceptions;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class AddOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<AddOrderItemCommand> validator)
    : ICommandHandler<AddOrderItemCommand, OrderDto>
{
    public async Task<OrderDto> HandleAsync(
        AddOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(command).ThrowIfInvalid();

        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.AddItem(command.ProductId, command.ProductName, command.UnitPrice, command.Quantity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
