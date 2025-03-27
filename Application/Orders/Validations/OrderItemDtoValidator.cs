using Application.DTOs;
using FluentValidation;

namespace Application.Orders.Validations
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDetailDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
