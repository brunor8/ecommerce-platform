using Ecommerce.Domain.Orders;

namespace Ecommerce.Application.Orders.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? CancelledAt,
    string? ReasonCancelled,
    decimal Total,
    IReadOnlyList<OrderItemDto> Items)
{
    public static OrderDto FromDomain(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.CreatedAt,
            order.UpdatedAt,
            order.CancelledAt,
            order.ReasonCancelled,
            order.Total,
            order.Items.Select(OrderItemDto.FromDomain).ToArray());
}
