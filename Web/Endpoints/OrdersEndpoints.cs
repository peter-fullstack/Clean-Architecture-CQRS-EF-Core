using Application.Commands;
using FluentValidation;
using MediatR;

namespace Web.Endpoints
{
    // Web/Endpoints/OrdersEndpoints.cs
    public static class OrdersEndpoints
    {
        public static void MapOrdersEndpoints(this WebApplication app)
        {
            app.MapPost("/orders", async (
                PlaceOrderCommand command,
                IMediator mediator,
                IValidator<PlaceOrderCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var orderId = await mediator.Send(command);
                return Results.Created($"/orders/{orderId}", orderId);
            });

            app.MapGet("/orders/{id}", async (
                Guid id,
                IMediator mediator) =>
            {
                //var query = new GetOrderByIdQuery(id);
                //var order = await mediator.Send(query);
                //return order is not null ? Results.Ok(order) : Results.NotFound();

                return Results.Ok();
            });
        }
    }
}
