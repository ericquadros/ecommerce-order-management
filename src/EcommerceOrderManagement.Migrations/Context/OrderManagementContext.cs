using EcommerceOrderManagement.Migrations.EFMappings;
using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Migrations.Context;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options)
        : base(options)
    {}
    
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new ProductMapping());
        modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
        modelBuilder.ApplyConfiguration(new OrderMapping());
        modelBuilder.ApplyConfiguration(new OrderItemMapping());
        modelBuilder.ApplyConfiguration(new CustomerMapping());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
                                 
        optionsBuilder.UseSqlServer(connectionString);
    }
}