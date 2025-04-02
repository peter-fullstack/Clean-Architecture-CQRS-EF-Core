// Infrastructure/Data/DesignTimeDbContextFactory.cs
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    //public AppDbContext CreateDbContext(string[] args)
    //{
    //    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

    //    // Using EF Core's in-memory database
    //    optionsBuilder.UseInMemoryDatabase(databaseName: "MyAppInMemoryDb");

    //    return new AppDbContext(optionsBuilder.Options);
    //}

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyApp;Trusted_Connection=True;");
        // or UseSqlite("Data Source=mydatabase.db")
        return new AppDbContext(optionsBuilder.Options);
    }
}