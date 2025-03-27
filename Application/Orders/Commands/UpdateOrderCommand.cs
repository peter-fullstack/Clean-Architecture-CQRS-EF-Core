using MediatR;

public record UpdateOrderStatusCommand : IRequest
{
    public Guid OrderId { get; init; }
    public OrderStatus NewStatus { get; init; }

    public class OrderStatus
    {
    }
}