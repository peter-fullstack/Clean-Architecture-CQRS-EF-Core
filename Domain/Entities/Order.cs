namespace Domain.Entities;

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

        if (_items.Any(i => i.ProductId == productId))
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Price = price;
                item.Quantity += quantity;
            }
        }
        else
        {
            _items.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = price
            });
        }
    }

    public decimal OrderTotal
    {
        get
        {
            return _items.Sum(i => i.Quantity * i.Price);
        }
    }
}