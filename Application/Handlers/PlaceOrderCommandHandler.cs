using Application.Commands;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IProductService _productService;

    public PlaceOrderCommandHandler(IUnitOfWork uow, IProductService productService)
    {
        _uow = uow;
        _productService = productService;
    }

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        // 1. Validate the Product Ids
        var productIds = cmd.Items.Select(i => i.ProductId).ToList();
        var validProducts = await _productService.ValidateProductsAsync(productIds);

        if (!validProducts.IsValid) 
        {
            throw new ApplicationValidationException($"ERROR: Validation for product Ids failed for new order.",  validProducts.Errors);
        }

        // 2. Map Command DTOs to domain entities
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = cmd.CustomerId
        };

        foreach (var item in cmd.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, item.Price);
        }

        // 3. Persist
        await _uow.Repository<Order>().AddAsync(order);
        await _uow.CommitAsync(ct);

        return order.Id;
    }
}