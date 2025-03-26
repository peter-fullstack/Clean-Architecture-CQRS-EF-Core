using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
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
        var productIds = cmd.Items.Select(i => i.ProductId).ToList();
        var validProducts = await _productService.ValidateProductsAsync(productIds);

        // 2. Map DTOs to domain entities
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Items = cmd.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };

        // 3. Persist
        await _uow.Repository<Order>().AddAsync(order);
        await _uow.CommitAsync(ct);

        return order.Id;
    }
}