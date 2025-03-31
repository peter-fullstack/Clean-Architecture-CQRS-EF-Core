using Application.Exceptions;
using Moq;

namespace Application.Tests.Orders.Commands;

public class PlaceOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesOrderWithCorrectPropertiesAndSaves()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var successValidationResult = PlaceOrderCommandHandlerTestHelper.CreateProductServiceValidationResult(true, new List<string>());
        var productService = PlaceOrderCommandHandlerTestHelper.CreateProductService(successValidationResult);

        var orderItems = PlaceOrderCommandHandlerTestHelper.CreateOrderItemList();
        var command = PlaceOrderCommandHandlerTestHelper.CreateValidOrderCommand(customerId, orderItems);
        var (handler, mockUow) = PlaceOrderCommandHandlerTestHelper.CreateTestHandler(productService.Object);

        // Act
        var orderId = await handler.Handle(command, CancellationToken.None);

        // Assert
        PlaceOrderCommandHandlerTestHelper.VerifyOrderCreation(mockUow, orderId, command);
    }

    [Fact]
    public async Task Handle_CommandWithInvalidProductId_ThrowsException()
    {
        // Arrange
        var errors = new List<string> { "Invalid SKU-123" };
        var customerId = Guid.NewGuid();
        var successValidationResult = PlaceOrderCommandHandlerTestHelper.CreateProductServiceValidationResult(false, errors);
        var productService = PlaceOrderCommandHandlerTestHelper.CreateProductService(successValidationResult);

        var orderItems = PlaceOrderCommandHandlerTestHelper.CreateOrderItemList();
        var command = PlaceOrderCommandHandlerTestHelper.CreateValidOrderCommand(customerId, orderItems);
        var (handler, mockUow) = PlaceOrderCommandHandlerTestHelper.CreateTestHandler(productService.Object);

        // Act and Assert
        await PlaceOrderCommandHandlerTestHelper.VerifyExceptionThrown<ApplicationValidationException>(
            () => handler.Handle(command, CancellationToken.None),
            errors);

        productService.Verify(
            p => p.ValidateProductsAsync(It.IsAny<List<Guid>>()),
            Times.Once,
            "Product Ids should be validated");
    }
}