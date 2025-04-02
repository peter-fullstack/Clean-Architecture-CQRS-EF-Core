using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using Testcontainers.MsSql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Web.Tests;
public abstract class WebApiTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();

    protected HttpClient Client { get; private set; }
    protected AppDbContext DbContext { get; private set; }

    protected WebApplicationFactory<Program> Factory { get; private set; }


    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var sqlServerConnectionString = _dbContainer.GetConnectionString();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Override production DB with TestContainers
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(sqlServerConnectionString));
                });
            });
        try
        {
            Client = Factory.CreateClient();
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }

        using (var scope = Factory.Services.CreateScope())
        {
            DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DbContext.Database.MigrateAsync();
        }
    }

    public async Task DisposeAsync()
    {
        //await _dbContainer.DisposeAsync();
        //await DbContext.DisposeAsync();
    }
}