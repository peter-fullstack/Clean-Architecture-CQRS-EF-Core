using Application.Commands;
using FluentValidation;

namespace Application.Orders.Validations;

public class CreateOrderValidator : AbstractValidator<PlaceOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}
