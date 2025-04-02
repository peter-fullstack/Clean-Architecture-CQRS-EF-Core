using Application.Orders.DTOs;
using MediatR;

namespace Application.Commands;

public record UpdateOrderCommand
(
    Guid OrderId,
    Guid CustomerId,
    List<OrderItemDetailDto> Items
) : IRequest<Guid>;