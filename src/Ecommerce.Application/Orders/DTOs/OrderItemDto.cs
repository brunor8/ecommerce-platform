using Ecommerce.Domain.Orders;

namespace Ecommerce.Application.Orders.DTOs;

public sealed record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice)
{
    public static OrderItemDto FromDomain(OrderItem orderItem) =>
        new (
            orderItem.Id,
            orderItem.ProductId,
            orderItem.ProductName,
            orderItem.UnitPrice,
            orderItem.Quantity,
            orderItem.TotalPrice);
}
