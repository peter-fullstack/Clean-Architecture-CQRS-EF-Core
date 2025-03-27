using Application.DTOs;

namespace Application.Orders.Services
{
    public class OrderPricingService
    {
        public decimal CalculateOrderTotal(List<OrderItemDetailDto> items)
        {
            return items.Sum(i => i.Quantity * i.Price);
        }
    }
}
