// Domain/Entities/Product.cs
using Domain.Exceptions;

namespace Domain.Entities;

public class Product
{
    private Product() { } // Private constructor for EF Core

    // Public constructor for domain logic
    public Product(Guid id, string name, string sku, decimal price, int initialStock = 0)
    {
        Id = id;
        SetName(name);
        SetSku(sku);
        SetPrice(price);
        Stock = initialStock;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Sku { get; private set; } // Stock Keeping Unit
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    // Domain Methods
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty", new List<string>());

        if (name.Length > 100)
            throw new DomainException("Product name exceeds 100 characters", new List<string>());

        Name = name;
    }

    public void SetSku(string sku)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(sku, @"^[A-Z0-9-]+$"))
            throw new DomainException("SKU must be alphanumeric with hyphens", new List<string>());

        Sku = sku;
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("Price must be positive", new List<string>());

        Price = price;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive", new List<string>());

        Stock += quantity;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive", new List<string>());

        if (Stock - quantity < 0)
            throw new DomainException("Insufficient stock", new List<string>()) ;

        Stock -= quantity;
    }
}