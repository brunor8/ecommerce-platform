namespace Ecommerce.Application.Orders.Commands;

public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);
