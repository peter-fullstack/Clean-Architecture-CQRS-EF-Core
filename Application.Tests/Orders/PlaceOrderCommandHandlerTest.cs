using Application.Exceptions;
using Moq;

namespace Application.Tests.Orders;

public class PlaceOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesOrderWithCorrectPropertiesAndSaves()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var successValidationResult = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateProductServiceValidationResult(true, new List<string>());
        var productService = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateProductService(successValidationResult);

        var orderItems = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateOrderItemList();
        var command = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateValidOrderCommand(customerId, orderItems);
        var (handler, mockUow) = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateTestHandler(productService.Object);

        // Act
        var orderId = await handler.Handle(command, CancellationToken.None);

        // Assert
        CreatesOrderWithCorrectPropertiesAndSavesHelper.VerifyOrderCreation(mockUow, orderId, command);
    }

    [Fact]
    public async Task Handle_CommandWithInvalidProductId_ThrowsException()
    {
        // Arrange
        var errors = new List<string> { "Invalid SKU-123" };
        var customerId = Guid.NewGuid();
        var successValidationResult = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateProductServiceValidationResult(false, errors);
        var productService = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateProductService(successValidationResult);

        var orderItems = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateOrderItemList();
        var command = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateValidOrderCommand(customerId, orderItems);
        var (handler, mockUow) = CreatesOrderWithCorrectPropertiesAndSavesHelper.CreateTestHandler(productService.Object);

        // Act and Assert
        await CreatesOrderWithCorrectPropertiesAndSavesHelper.VerifyExceptionThrown<ApplicationValidationException>(
            () => handler.Handle(command, CancellationToken.None),
            errors);

        productService.Verify(
            p => p.ValidateProductsAsync(It.IsAny<List<Guid>>()),
            Times.Once,
            "Product Ids should be validated");
    }
}