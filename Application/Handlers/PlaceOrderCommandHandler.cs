// Application/Orders/Commands/PlaceOrder/PlaceOrderCommandHandler.cs
using MediatR;
using Domain.Entities;
using Domain.ValueObjects;
using Application.Interfaces;
using Application.Commands;
using Microsoft.Extensions.Logging;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Orders.Commands.PlaceOrder;

public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderResult>
{
    private readonly IRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(
        IRepository orderRepository,
        IUnitOfWork unitOfWork,
        IProductService productService,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _productService = productService;
        _logger = logger;
    }

    public async Task<OrderResult> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate products exist and get current prices
            var productValidations = await ValidateProducts(request.Items);
            if (!productValidations.IsValid)
            {
                throw new ApplicationException("Product validation failed: " +
                    string.Join(", ", productValidations.Errors));
            }

            // 2. Create domain entity
            var order = new Order(
                OrderId.New(),
                new CustomerId(request.CustomerId),
                request.OrderDate);

            // 3. Add items with domain logic
            foreach (var item in request.Items)
            {
                order.AddItem(
                    new ProductId(item.ProductId),
                    productValidations.Prices[item.ProductId],
                    item.Quantity);
            }

            // 4. Process payment (domain service)
            order.ProcessPayment(request.PaymentMethod);

            // 5. Persist
            await _orderRepository.AddAsync(order);
            await _unitOfWork.CommitAsync(cancellationToken);

            // 6. Raise domain events if needed
            // (handled by MediatR or separate event dispatcher)

            _logger.LogInformation("Order {OrderId} placed successfully", order.Id);

            return new OrderResult(order.Id.Value, order.Total, order.Status.ToString());
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain error placing order");
            throw; // Let the global exception handler catch this
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing order");
            throw new ApplicationException("Failed to place order", ex);
        }
    }

    private async Task<ProductValidationResult> ValidateProducts(
        IEnumerable<OrderItemDto> items)
    {
        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        return await _productService.ValidateProductsAsync(productIds);
    }
}