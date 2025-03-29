// Tests/Domain/Entities/OrderTests.cs
using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace Domain.Tests.Entities;

public class OrderTests
{
    private readonly Guid _testProductId = Guid.NewGuid();
    private const decimal _testPrice = 10.99m;

    [Fact]
    public void AddItem_NewProduct_AddsToItemsList()
    {
        // Arrange
        var order = new Order();
        int initialItemCount = order.Items.Count;

        // Act
        order.AddItem(_testProductId, quantity: 2, _testPrice);

        // Assert
        Assert.Equal(initialItemCount + 1, order.Items.Count);
        var item = order.Items.Last();
        Assert.Equal(_testProductId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(_testPrice, item.Price);
    }

    [Fact]
    public void AddItem_ExistingProduct_UpdatesQuantityAndPrice()
    {
        // Arrange
        var order = new Order();
        order.AddItem(_testProductId, quantity: 2, _testPrice);

        // Act
        order.AddItem(_testProductId, quantity: 3, 9.99m);

        // Assert
        Assert.Single(order.Items);
        var item = order.Items.First();
        Assert.Equal(5, item.Quantity); // 2 + 3
        Assert.Equal(9.99m, item.Price); // Price updated to latest value
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_InvalidQuantity_ThrowsDomainException(int invalidQuantity)
    {
        // Arrange
        var order = new Order();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            order.AddItem(_testProductId, invalidQuantity, _testPrice));
        Assert.Contains("Quantity must be positive", ex.Message);
    }

    [Fact]
    public void OrderTotal_CalculatesCorrectSum()
    {
        // Arrange
        var order = new Order();
        order.AddItem(Guid.NewGuid(), quantity: 2, price: 10m); // 20
        order.AddItem(Guid.NewGuid(), quantity: 1, price: 5.5m); // 5.5

        // Act
        var total = order.OrderTotal;

        // Assert
        Assert.Equal(25.5m, total);
    }

    [Fact]
    public void OrderTotal_EmptyOrder_ReturnsZero()
    {
        // Arrange
        var order = new Order();

        // Act
        var total = order.OrderTotal;

        // Assert
        Assert.Equal(0m, total);
    }

    [Fact]
    public void Items_Property_ReturnsCopyOfCollection()
    {
        // Arrange
        var order = new Order();
        order.AddItem(_testProductId, 1, _testPrice);

        // Act
        var items = order.Items;
        items.Clear();

        // Assert
        Assert.NotEmpty(order.Items); // Original collection unchanged
    }
}