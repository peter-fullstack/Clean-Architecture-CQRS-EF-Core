public class OrderModel : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public DateTime OrderDate { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.Price * item.Quantity);

    public OrderModel(Guid id, CustomerId customerId, DateTime orderDate)
        : base(id)
    {
        CustomerId = customerId;
        OrderDate = orderDate;
    }

    public void AddItem(ProductId productId, decimal price, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, price, quantity));
        }

        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
    }

    // EF Core constructor
    private Order() { }
}

public class OrderItem : Entity<int>
{
    public ProductId ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    internal OrderItem(ProductId productId, decimal price, int quantity)
    {
        ProductId = productId;
        Price = price;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    // EF Core constructor
    private OrderItem() { }
}

// Example domain event
public record OrderItemAddedEvent(Guid OrderId, ProductId ProductId, int Quantity)
    : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;
}