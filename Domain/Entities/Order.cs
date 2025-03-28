using Domain.Entities;

public class Order
{
    private List<OrderItem> _items = new();
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items
    {
        get
        {
            return _items;
        }
    }

    // Domain logic
    public void AddItem(Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be positive");
        _items.Add(new OrderItem
        {
            ProductId = productId, 
            Quantity = quantity, 
            Price = price 
        });
    }
}