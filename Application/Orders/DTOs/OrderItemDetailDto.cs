namespace Application.Orders.DTOs
{
    public class OrderItemDetailDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
