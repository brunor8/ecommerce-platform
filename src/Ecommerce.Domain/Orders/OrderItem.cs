using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Orders;

public class OrderItem
{
    private OrderItem() { } // EF Core

    internal OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            throw new DomainException("Unit price cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("Product name is required.");
        }

        if (productId == Guid.Empty)
        {
            throw new DomainException("Invalid product.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        Quantity += quantity;
    }

}