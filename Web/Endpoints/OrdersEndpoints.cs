using Application.Commands;
using Application.Orders.Queries;
using FluentValidation;
using MediatR;

namespace Web.Endpoints
{
    public static class OrdersEndpoints
    {
        public static void MapOrdersEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("orders")
                .WithTags("Orders")
                .WithOpenApi();

            group.MapPost("/", async (
                PlaceOrderCommand command,
                IMediator mediator,
                IValidator<PlaceOrderCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)         
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var orderId = await mediator.Send(command);
                return Results.Created($"/orders/{orderId}", orderId);
            })
            .WithName("PlaceOrder")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

            group.MapGet("/{id}", async (
                Guid id,
                IMediator mediator) =>
            {
                var query = new GetOrderByIdQuery(id);
                var order = await mediator.Send(query);
                return order is not null ? Results.Ok(order) : Results.NotFound();
            })
            .WithName("GetOrder")
            .Produces<OrderDetailDto>()
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}