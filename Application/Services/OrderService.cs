using MyProject.Domain.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IRepository _orderRepository;

        public OrderService(IRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task PlaceOrder(PlaceOrderCommand command)
        {
            // Use case implementation
        }
    }
}
