using Application.Commands;
using FluentValidation;

namespace Application.Orders.Validations;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}
