using Application.Commands;
using Application.Interfaces;
using Application.Orders.Validations;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Web.Endpoints;

namespace Web;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register MediatR
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PlaceOrderCommand).Assembly));

        // Register FluentValidation
        builder.Services.AddScoped<IValidator<PlaceOrderCommand>, CreateOrderValidator>();
        builder.Services.AddScoped<IValidator<UpdateOrderCommand>, UpdateOrderStatusValidator>();

        // Register other services
        builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IProductService, ProductService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            // Apply pending migrations
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }
        }

        app.UseHttpsRedirection();

        // Register endpoints
        app.MapOrdersEndpoints();

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                if (context.Features.Get<IExceptionHandlerFeature>()?.Error is ValidationException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Title = "Validation Error",
                        Status = context.Response.StatusCode,
                        Errors = ex.Errors.GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    });
                }
            });
        });

        app.Run();
    }
}