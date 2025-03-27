using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record PlaceOrderCommand
(
    Guid CustomerId,
    List<OrderItemDetailDto> Items
) : IRequest<Guid>;