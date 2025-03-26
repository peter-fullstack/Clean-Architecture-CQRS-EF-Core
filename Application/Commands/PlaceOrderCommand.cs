using MediatR;

namespace Application.Commands;

public record PlaceOrderCommand : IRequest<OrderResult>
{
    public Guid CustomerId { get; init; }
    public DateTime OrderDate { get; init; } = DateTime.UtcNow;
    public string PaymentMethod { get; init; } = "CreditCard";
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}

public record OrderResult(Guid OrderId, decimal Total, string Status);