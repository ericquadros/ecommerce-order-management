using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Migrations.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderManagementMigrationsDbContext>
{
    public OrderManagementMigrationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderManagementMigrationsDbContext>();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
        Console.WriteLine(connectionString);
        optionsBuilder.UseSqlServer(connectionString);

        return new OrderManagementMigrationsDbContext(optionsBuilder.Options);
    }
}