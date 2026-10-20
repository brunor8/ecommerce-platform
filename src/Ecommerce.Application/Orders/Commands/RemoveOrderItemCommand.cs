namespace Ecommerce.Application.Orders.Commands;

public sealed record RemoveOrderItemCommand(Guid OrderId, Guid ProductId);
