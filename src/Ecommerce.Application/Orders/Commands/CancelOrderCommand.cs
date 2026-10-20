namespace Ecommerce.Application.Orders.Commands;

public sealed record CancelOrderCommand(Guid OrderId, string Motive);
