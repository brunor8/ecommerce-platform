using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.Exceptions;

namespace Ecommerce.Application.Orders.Handlers;

public sealed class ApproveOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<ApproveOrderCommand> validator)
    : ICommandHandler<ApproveOrderCommand>
{
    public async Task HandleAsync(
        ApproveOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        validator.Validate(command).ThrowIfInvalid();

        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.Approve();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
