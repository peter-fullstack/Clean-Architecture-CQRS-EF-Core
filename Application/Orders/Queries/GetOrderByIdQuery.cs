using MediatR;

namespace Application.Orders.Queries
{
    // Application/Orders/Queries/GetOrderByIdQuery.cs
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailDto>;

    public record OrderDetailDto(
        Guid Id,
        string CustomerName,
        List<OrderItemDetailDto> Items,
        decimal Total);

    public record OrderItemDetailDto(
        string ProductName,
        int Quantity,
        decimal UnitPrice);
}
