using Application.Orders.DTOs;
using Docker.DotNet.Models;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Web.Tests.Orders;

public class OrdersEndpointsTests : WebApiTestBase
{
    [Fact]
    public async Task POST_orders_Returns201Created_WithValidRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Products.AddAsync(new Product(productId, "Test", "AU-9809", 10m, 20));
            await dbContext.SaveChangesAsync();
        }

        var request = new
        {
            CustomerId = Guid.NewGuid(),
            Items = new[] { new { ProductId = productId, Quantity = 2, Price = 10m } }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var orderId = await response.Content.ReadFromJsonAsync<Guid>();
        orderId.Should().NotBeEmpty();

        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbOrder = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            dbOrder.Should().NotBeNull();
            dbOrder!.Items.Should().Contain(i => i.ProductId == productId && i.Quantity == 2);
        }
    }

    [Fact]
    public async Task GET_orders_id_ReturnsOrderDetails()
    {
        // Arrange - Seed data directly
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = { new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1, Price = 15m } }
        };
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/orders/{order.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OrderDetailDto>();

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.Total.Should().Be(15m);
    }

    [Fact]
    public async Task POST_orders_Returns400_WhenProductDoesntExist()
    {
        // Arrange
        var request = new
        {
            CustomerId = Guid.NewGuid(),
            Items = new[] { new { ProductId = Guid.NewGuid(), Quantity = 1, Price = 10m } }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Invalid product");
    }
}