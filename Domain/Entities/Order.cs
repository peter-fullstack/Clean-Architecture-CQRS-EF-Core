using Domain.Entities;
using Domain.ValueObjects;

// Domain/Entities/Order.cs
public class Order : AggregateRoot<OrderId>
{
    // ... other members ...

    public void AddItem(OrderId productId, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be positive");

        if (price <= 0)
            throw new Exception("Price must be positive");

        var item = new OrderItem(Id, productId, price, quantity);
        _items.Add(item);

        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity, price));
    }

    public void ProcessPayment(string paymentMethod)
    {
        // Domain logic for payment processing
    }
}