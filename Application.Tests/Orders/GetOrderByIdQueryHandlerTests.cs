using Application.Orders.DTOs;
using Application.Orders.Queries;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Linq;

namespace Application.Tests.Orders.Queries;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ReturnsCorrectDto()
    {
        // Arrange
        var (handler, order) = CreateHandlerWithOrder(
            orderId: Guid.NewGuid(),
            customerName: "Bob Newbold",
            items: new List<OrderItem>
            {
                new OrderItem { ProductId = Guid.NewGuid(), Quantity = 2, Price = 10m },
                new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1, Price = 5m }
            });

        var query = new GetOrderByIdQuery(OrderId: order.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25m, result.Total); // 2*10 + 1*5
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Bob Newbold", result.CustomerName);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ReturnsNull()
    {
        // Arrange
        var (handler, _) = CreateHandlerWithOrder(null);
        var query = new GetOrderByIdQuery(OrderId: Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_EmptyOrder_ReturnsZeroTotal()
    {
        // Arrange
        var (handler, order) = CreateHandlerWithOrder(
            items: new List<OrderItem>());

        var query = new GetOrderByIdQuery(OrderId: order.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0m, result!.Total);
    }

    // Helper Methods
    private static (GetOrderByIdQueryHandler handler, Order order)
        CreateHandlerWithOrder(
            List<OrderItem>? items = null,
            Guid? orderId = null,
            string? customerName = null)
    {
        var mockRepo = new Mock<IRepository<Order>>();
        var order = new Order
        {
            Id = orderId ?? Guid.NewGuid(),
        };

        if (items != null)
        {
            foreach (var item in items)
            {
                order.AddItem(item.ProductId, item.Quantity, item.Price);
            }
        }

        mockRepo.Setup(r => r.GetByIdAsync(It.Is<Guid>(id => id == order.Id)))
            .ReturnsAsync(order);
        mockRepo.Setup(r => r.GetByIdAsync(It.Is<Guid>(id => id != order.Id)))
            .ReturnsAsync((Order?)null);

        var handler = new GetOrderByIdQueryHandler(mockRepo.Object);
        return (handler, order);
    }
}