using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Orders.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
    {
        private readonly IRepository<Order> _repo;
        public GetOrderByIdQueryHandler(IRepository<Order> repo) => _repo = repo;

        public async Task<OrderDetailDto?> Handle(
            GetOrderByIdQuery query,
            CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(query.OrderId);
            if (order is null) return null;

            return new OrderDetailDto
            (
                order.Id,
                "Bob Newbold",
                order.Items.Select(i => new OrderItemDetailDto(
                    i.ProductId.ToString(),
                    i.Quantity,
                    i.Price)).ToList(),
                order.Items.Sum(i => i.Quantity * i.Price)
             );
        }
    }
}
