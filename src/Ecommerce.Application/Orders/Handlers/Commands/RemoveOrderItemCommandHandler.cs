using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.Exceptions;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class RemoveOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<RemoveOrderItemCommand> validator)
    : ICommandHandler<RemoveOrderItemCommand>
{
    public async Task HandleAsync(
        RemoveOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(command).ThrowIfInvalid();

        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.RemoveItem(command.ProductId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
