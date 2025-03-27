using Application.DTOs;

namespace Application.Orders.DTOs
{
    public record OrderDetailDto
    (
        Guid Id,
        string CustomerName,
        List<OrderItemDetailDto> Items,
        decimal Total
    );
}
