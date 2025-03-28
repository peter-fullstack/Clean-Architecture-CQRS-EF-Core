// Tests/Unit/Orders/CreateOrderCommandHandlerTests.cs
using Application.Commands;
using Application.DTOs;
using Application.Interfaces;
using Application.Orders.DTOs;
using Domain.Interfaces;
using Moq;

public class PlaceOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesOrder()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Order>>();
        mockRepo.Setup(u => u.AddAsync(It.IsAny<Order>()));

        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Repository<Order>())
            .Returns(mockRepo.Object);

        var productValidationResult = new ProductValidationResult(
            true, 
            new Dictionary<Guid, decimal>(), 
            new List<string>());
        
        var productService = new Mock<IProductService>();
        productService.Setup(p => p.ValidateProductsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(productValidationResult);

        var handler = new PlaceOrderCommandHandler(
            mockUow.Object,
            productService.Object);

        var command = new PlaceOrderCommand(
            CustomerId: Guid.NewGuid(),
            Items: new List<OrderItemDetailDto> 
            { 
                new OrderItemDetailDto
                { 
                    ProductId = Guid.NewGuid(), 
                    Price = 20M,
                    Quantity = 2 
                } 
            });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUow.Verify(u => u.Repository<Order>().AddAsync(It.IsAny<Order>()), Times.Once);
        mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }
}