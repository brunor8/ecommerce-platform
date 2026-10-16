namespace Ecommerce.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items = new();
    private Order() { } // EF Core


    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? ReasonCancelled { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.TotalPrice);

    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer id is required.", nameof(customerId));

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("It is not possible to change items in a non-pending order.");

        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be approved.");

        if (_items.Count == 0)
            throw new InvalidOperationException("An order must contain at least one item to be approved.");

        Status = OrderStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string motive)
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Approved))
            throw new InvalidOperationException("The order cannot be cancelled in its current state.");

        if (string.IsNullOrWhiteSpace(motive))
            throw new InvalidOperationException("A reason for cancellation must be provided.");

        if (motive.Length > 200)
            throw new InvalidOperationException("The reason for cancellation cannot exceed 200 characters.");

        Status = OrderStatus.Cancelled;
        ReasonCancelled = motive;
        CancelledAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("It is not possible to remove items from a non-pending order.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new ArgumentException("Product not found in the order.", nameof(productId));

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }
}