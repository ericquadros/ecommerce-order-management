using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Migrations.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderManagementDbContext>
{
    public OrderManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderManagementDbContext>();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
        Console.WriteLine(connectionString);
        optionsBuilder.UseSqlServer(connectionString);

        return new OrderManagementDbContext(optionsBuilder.Options);
    }
}