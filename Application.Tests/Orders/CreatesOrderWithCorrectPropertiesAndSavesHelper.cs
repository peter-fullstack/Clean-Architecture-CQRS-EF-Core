using Application.Commands;
using Application.Exceptions;
using Application.Interfaces;
using Application.Orders.DTOs;
using Domain.Interfaces;
using Moq;

namespace Application.Tests.Orders;

public class CreatesOrderWithCorrectPropertiesAndSavesHelper
{
    public static (PlaceOrderCommandHandler handler, Mock<IUnitOfWork> mockUow) CreateTestHandler(
        IProductService productService)
    {
        var mockUow = new Mock<IUnitOfWork>();
        var mockRepo = new Mock<IRepository<Order>>();

        mockUow.Setup(u => u.Repository<Order>()).Returns(mockRepo.Object);
        mockUow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);



        return (new PlaceOrderCommandHandler(mockUow.Object, productService), mockUow);
    }

    public static PlaceOrderCommand CreateValidOrderCommand(
        Guid customerId,
        List<OrderItemDetailDto> items)
    {
        return new PlaceOrderCommand(
            customerId,
            items);
    }

    public static List<OrderItemDetailDto> CreateOrderItemList()
    {
        return new List<OrderItemDetailDto>
    {
        new()
        {
            ProductId = Guid.NewGuid(),
            Price = 20M,
            Quantity = 2
        }
    };
    }

    public static void VerifyOrderCreation(
        Mock<IUnitOfWork> mockUow,
        Guid generatedOrderId,
        PlaceOrderCommand command)
    {
        // Verify order was created with correct properties
        mockUow.Verify(u => u.Repository<Order>().AddAsync(
            It.Is<Order>(o =>
                o.Id != Guid.Empty &&
                o.CustomerId == command.CustomerId &&
                o.Items.Count == command.Items.Count &&
                o.Items.All(i =>
                    command.Items.Any(ci =>
                        ci.ProductId == i.ProductId &&
                        ci.Quantity == i.Quantity)))),
            Times.Once,
            "Order should be created with matching command properties");

        // Verify persistence
        mockUow.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once,
            "Changes should be committed");

        // Verify output
        Assert.NotEqual(Guid.Empty, generatedOrderId);
    }

    public static Mock<IProductService> CreateProductService(ProductValidationResult validationResult)
    {
        var mock = new Mock<IProductService>();
        mock.Setup(p => p.ValidateProductsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(validationResult);
        return mock;
    }

    public static ProductValidationResult CreateProductServiceValidationResult(bool success, List<string> errors)
    {
        return new ProductValidationResult(
            IsValid: success,
            Prices: new Dictionary<Guid, decimal>(),
            Errors: errors);
    }

    public static ProductValidationResult CreateFailedValidationResult(List<string> errors)
    => new(false, new Dictionary<Guid, decimal>(), errors);

    public static async Task VerifyExceptionThrown<TException>(
        Func<Task> testAction,
        List<string>? expectedErrors = null)
        where TException : Exception
    {
        var exception = await Assert.ThrowsAsync<TException>(testAction);

        if (expectedErrors != null && exception is ApplicationValidationException domainEx)
        {
            Assert.Equal(expectedErrors, domainEx.Errors);
        }
    }
}
